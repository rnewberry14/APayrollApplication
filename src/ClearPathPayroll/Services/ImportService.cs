using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ClearPathPayroll.Services;

public class ImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly PayrollDbContext _context;
    private readonly IEnumerable<IImportFileParser> _parsers;

    public ImportService(PayrollDbContext context, IEnumerable<IImportFileParser> parsers)
    {
        _context = context;
        _parsers = parsers;
    }

    public async Task<ImportFileParseResult> ParseFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var parser = GetParser(fileName);
        return await parser.ParseAsync(stream, fileName, cancellationToken);
    }

    public async Task<ImportBatch> CreateStagedBatchAsync(
        string fileName,
        ImportType importType,
        ImportFileParseResult parseResult,
        IEnumerable<ImportMappingInput> mappings,
        bool demoModeWarningAcknowledged,
        string? createdByUserId = null,
        CancellationToken cancellationToken = default)
    {
        ValidateFileName(fileName);
        var mappingList = mappings.ToList();
        ValidateMappings(mappingList);

        var batch = new ImportBatch
        {
            FileName = Path.GetFileName(fileName),
            ImportType = importType,
            FileFormat = parseResult.FileFormat,
            Status = ImportBatchStatus.Mapped,
            RowCount = parseResult.Rows.Count,
            DemoModeWarningAcknowledged = demoModeWarningAcknowledged,
            CreatedByUserId = string.IsNullOrWhiteSpace(createdByUserId) ? "local-prototype-user" : createdByUserId.Trim(),
            CreatedAt = DateTime.UtcNow,
            Mappings = mappingList.Select(mapping => new ImportMapping
            {
                SourceColumn = mapping.SourceColumn.Trim(),
                TargetField = mapping.TargetField.Trim(),
                IsRequired = mapping.IsRequired,
                CreatedAt = DateTime.UtcNow
            }).ToList(),
            Rows = parseResult.Rows.Select((row, index) => new ImportRow
            {
                RowNumber = index + 2,
                RowDataJson = JsonSerializer.Serialize(row, JsonOptions),
                Status = ImportRowStatus.PendingReview,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        _context.ImportBatches.Add(batch);
        await _context.SaveChangesAsync(cancellationToken);

        await ValidateBatchAsync(batch.ImportBatchId, cancellationToken);
        return await GetBatchAsync(batch.ImportBatchId, cancellationToken)
            ?? throw new InvalidOperationException("The import batch could not be loaded after staging.");
    }

    public async Task<ImportBatch?> GetBatchAsync(int importBatchId, CancellationToken cancellationToken = default)
    {
        return await _context.ImportBatches
            .Include(batch => batch.Mappings)
            .Include(batch => batch.Rows)
            .Include(batch => batch.Errors)
            .FirstOrDefaultAsync(batch => batch.ImportBatchId == importBatchId, cancellationToken);
    }

    public async Task<ImportBatch> ValidateBatchAsync(int importBatchId, CancellationToken cancellationToken = default)
    {
        var batch = await GetBatchAsync(importBatchId, cancellationToken)
            ?? throw new ValidationException("Import batch was not found.");

        _context.ImportErrors.RemoveRange(batch.Errors);

        var errors = new List<ImportError>();
        var requiredMappings = batch.Mappings.Where(mapping => mapping.IsRequired).ToList();

        if (batch.Mappings.Count == 0)
        {
            errors.Add(CreateBatchError(batch.ImportBatchId, "MissingMapping", "At least one column mapping is required."));
        }

        foreach (var row in batch.Rows)
        {
            var rowValues = DeserializeRow(row.RowDataJson);
            var rowErrors = new List<string>();

            foreach (var mapping in requiredMappings)
            {
                if (!rowValues.TryGetValue(mapping.SourceColumn, out var value) || string.IsNullOrWhiteSpace(value))
                {
                    errors.Add(new ImportError
                    {
                        ImportBatchId = batch.ImportBatchId,
                        ImportRowId = row.ImportRowId,
                        RowNumber = row.RowNumber,
                        ColumnName = mapping.SourceColumn,
                        ErrorCode = "RequiredValueMissing",
                        Message = $"Required mapped field '{mapping.TargetField}' is missing a value.",
                        CreatedAt = DateTime.UtcNow
                    });
                    rowErrors.Add(mapping.TargetField);
                }
            }

            row.Status = rowErrors.Count == 0 ? ImportRowStatus.Valid : ImportRowStatus.Error;
            row.ErrorSummary = rowErrors.Count == 0 ? null : $"Missing required fields: {string.Join(", ", rowErrors)}";
        }

        if (errors.Count > 0)
        {
            _context.ImportErrors.AddRange(errors);
        }

        batch.ValidRowCount = batch.Rows.Count(row => row.Status == ImportRowStatus.Valid);
        batch.ErrorCount = errors.Count;
        batch.Status = errors.Count == 0 ? ImportBatchStatus.ReadyForConfirmation : ImportBatchStatus.ValidationFailed;
        batch.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return batch;
    }

    public async Task<ImportBatch> ConfirmImportAsync(int importBatchId, CancellationToken cancellationToken = default)
    {
        var batch = await GetBatchAsync(importBatchId, cancellationToken)
            ?? throw new ValidationException("Import batch was not found.");

        if (batch.Status != ImportBatchStatus.ReadyForConfirmation)
        {
            throw new ValidationException("Import batch must be validated without blocking errors before confirmation.");
        }

        if (batch.ErrorCount > 0 || batch.Rows.Any(row => row.Status == ImportRowStatus.Error))
        {
            throw new ValidationException("Import batch has validation errors that block confirmation.");
        }

        foreach (var row in batch.Rows)
        {
            row.Status = ImportRowStatus.Imported;
            row.ImportedAt = DateTime.UtcNow;
        }

        batch.Status = ImportBatchStatus.Imported;
        batch.ConfirmedAt = DateTime.UtcNow;
        batch.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return batch;
    }

    public IImportFileParser GetParser(string fileName)
    {
        var parser = _parsers.FirstOrDefault(candidate => candidate.CanParse(fileName));
        if (parser == null)
        {
            throw new NotSupportedException("Supported import file types are CSV, tab-delimited text, and .xlsx workbooks.");
        }

        return parser;
    }

    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ValidationException("A local import file name is required.");
        }
    }

    private static void ValidateMappings(IEnumerable<ImportMappingInput> mappings)
    {
        foreach (var mapping in mappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.SourceColumn) || string.IsNullOrWhiteSpace(mapping.TargetField))
            {
                throw new ValidationException("Each import mapping requires a source column and target field.");
            }
        }
    }

    private static ImportError CreateBatchError(int importBatchId, string errorCode, string message)
    {
        return new ImportError
        {
            ImportBatchId = importBatchId,
            ErrorCode = errorCode,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Dictionary<string, string> DeserializeRow(string rowDataJson)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(rowDataJson, JsonOptions)
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}

public class ImportMappingInput
{
    public string SourceColumn { get; set; } = string.Empty;

    public string TargetField { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
}
