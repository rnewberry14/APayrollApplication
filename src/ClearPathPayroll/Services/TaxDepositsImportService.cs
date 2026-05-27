using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;

namespace ClearPathPayroll.Services;

public class TaxDepositsImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static readonly string[] RequiredTargetFields =
    {
        "CompanyIdentifier",
        "DepositDate",
        "TaxPeriodStart",
        "TaxPeriodEnd",
        "TaxType",
        "Agency",
        "Amount",
        "PaymentMethod"
    };

    public static readonly string[] OptionalTargetFields =
    {
        "ConfirmationNumber",
        "Notes"
    };

    public static readonly string[] AllTargetFields = RequiredTargetFields.Concat(OptionalTargetFields).ToArray();

    private readonly PayrollDbContext _context;
    private readonly ImportService _importService;

    public TaxDepositsImportService(PayrollDbContext context, ImportService importService)
    {
        _context = context;
        _importService = importService;
    }

    public Task<ImportFileParseResult> ParseFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        return _importService.ParseFileAsync(stream, fileName, cancellationToken);
    }

    public async Task<TaxDepositsImportValidationResult> ValidateAsync(
        string fileName,
        ImportFileParseResult parseResult,
        IEnumerable<ImportMappingInput> mappings,
        bool reviewAcknowledged,
        CancellationToken cancellationToken = default)
    {
        var normalizedMappings = NormalizeMappings(mappings).ToList();
        foreach (var mapping in normalizedMappings)
        {
            if (RequiredTargetFields.Contains(mapping.TargetField, StringComparer.OrdinalIgnoreCase))
            {
                mapping.IsRequired = true;
            }
        }

        var batch = await _importService.CreateStagedBatchAsync(
            fileName,
            ImportType.TaxDepositsOnly,
            parseResult,
            normalizedMappings,
            reviewAcknowledged,
            cancellationToken: cancellationToken);

        return await ValidateStagedBatchAsync(batch.ImportBatchId, cancellationToken);
    }

    public async Task<TaxDepositsImportConfirmationResult> ConfirmAsync(
        int importBatchId,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateStagedBatchAsync(importBatchId, cancellationToken);
        if (!validation.CanImport)
        {
            throw new ValidationException("Tax deposit import has validation errors that block confirmation.");
        }

        var batch = validation.Batch;
        var deposits = new List<TaxDepositRecord>();

        foreach (var row in batch.Rows.OrderBy(row => row.RowNumber))
        {
            var parsed = ParseDepositRow(batch.Mappings.ToList(), DeserializeRow(row.RowDataJson));
            var company = await ResolveCompanyAsync(parsed.CompanyIdentifier, cancellationToken)
                ?? throw new ValidationException("CompanyIdentifier did not match a local company.");

            deposits.Add(new TaxDepositRecord
            {
                CompanyId = company.CompanyId,
                DepositDate = parsed.DepositDate,
                TaxPeriodStart = parsed.TaxPeriodStart,
                TaxPeriodEnd = parsed.TaxPeriodEnd,
                TaxType = parsed.TaxType,
                Agency = parsed.Agency,
                Amount = parsed.Amount,
                ConfirmationNumber = parsed.ConfirmationNumber,
                PaymentMethod = parsed.PaymentMethod,
                Notes = parsed.Notes,
                RecordSource = "User-entered deposit record",
                ImportBatchId = batch.ImportBatchId,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.TaxDepositRecords.AddRange(deposits);
        await _context.SaveChangesAsync(cancellationToken);

        var confirmed = await _importService.ConfirmImportAsync(importBatchId, cancellationToken);
        return new TaxDepositsImportConfirmationResult
        {
            Batch = confirmed,
            ImportedDepositCount = deposits.Count
        };
    }

    private async Task<TaxDepositsImportValidationResult> ValidateStagedBatchAsync(
        int importBatchId,
        CancellationToken cancellationToken)
    {
        var batch = await _importService.GetBatchAsync(importBatchId, cancellationToken)
            ?? throw new ValidationException("Import batch was not found.");

        var mappings = batch.Mappings.ToList();
        var mappedTargets = new HashSet<string>(mappings.Select(mapping => mapping.TargetField), StringComparer.OrdinalIgnoreCase);
        var errors = new List<ImportError>();

        foreach (var required in RequiredTargetFields)
        {
            if (!mappedTargets.Contains(required))
            {
                errors.Add(CreateBatchError(batch.ImportBatchId, "RequiredMappingMissing", $"Required tax deposit field '{required}' is not mapped."));
            }
        }

        foreach (var row in batch.Rows.OrderBy(row => row.RowNumber))
        {
            var rowErrors = ValidateRow(batch.ImportBatchId, row, mappings, DeserializeRow(row.RowDataJson));
            errors.AddRange(rowErrors);
            row.Status = rowErrors.Count == 0 ? ImportRowStatus.Valid : ImportRowStatus.Error;
            row.ErrorSummary = rowErrors.Count == 0 ? null : "Tax deposit row has validation errors.";
        }

        _context.ImportErrors.RemoveRange(batch.Errors);
        if (errors.Count > 0)
        {
            _context.ImportErrors.AddRange(errors);
        }

        batch.ValidRowCount = batch.Rows.Count(row => row.Status == ImportRowStatus.Valid);
        batch.ErrorCount = errors.Count;
        batch.Status = errors.Count == 0 ? ImportBatchStatus.ReadyForConfirmation : ImportBatchStatus.ValidationFailed;
        batch.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var refreshed = await _importService.GetBatchAsync(importBatchId, cancellationToken)
            ?? throw new ValidationException("Import batch was not found after validation.");

        return new TaxDepositsImportValidationResult
        {
            Batch = refreshed,
            CanImport = refreshed.Status == ImportBatchStatus.ReadyForConfirmation && refreshed.ErrorCount == 0
        };
    }

    private List<ImportError> ValidateRow(
        int importBatchId,
        ImportRow row,
        List<ImportMapping> mappings,
        Dictionary<string, string> values)
    {
        var errors = new List<ImportError>();
        TaxDepositImportRow? parsed = null;

        try
        {
            parsed = ParseDepositRow(mappings, values);
        }
        catch (ValidationException ex)
        {
            errors.Add(CreateRowError(importBatchId, row, "Row", "InvalidTaxDepositRow", ex.Message));
        }

        if (parsed == null)
        {
            return errors;
        }

        if (ResolveCompany(parsed.CompanyIdentifier) == null)
        {
            errors.Add(CreateRowError(importBatchId, row, "CompanyIdentifier", "CompanyNotFound", "CompanyIdentifier did not match a local company."));
        }

        if (parsed.TaxPeriodEnd < parsed.TaxPeriodStart)
        {
            errors.Add(CreateRowError(importBatchId, row, "TaxPeriodEnd", "InvalidTaxPeriod", "TaxPeriodEnd cannot be before TaxPeriodStart."));
        }

        if (parsed.Amount <= 0)
        {
            errors.Add(CreateRowError(importBatchId, row, "Amount", "InvalidAmount", "Amount must be greater than zero."));
        }

        return errors;
    }

    private static TaxDepositImportRow ParseDepositRow(List<ImportMapping> mappings, Dictionary<string, string> values)
    {
        string Get(string targetField) => GetMappedValue(mappings, values, targetField);

        return new TaxDepositImportRow
        {
            CompanyIdentifier = RequiredText(Get("CompanyIdentifier"), "CompanyIdentifier"),
            DepositDate = RequiredDate(Get("DepositDate"), "DepositDate"),
            TaxPeriodStart = RequiredDate(Get("TaxPeriodStart"), "TaxPeriodStart"),
            TaxPeriodEnd = RequiredDate(Get("TaxPeriodEnd"), "TaxPeriodEnd"),
            TaxType = RequiredText(Get("TaxType"), "TaxType"),
            Agency = RequiredText(Get("Agency"), "Agency"),
            Amount = RequiredMoney(Get("Amount"), "Amount"),
            ConfirmationNumber = EmptyToNull(Get("ConfirmationNumber")),
            PaymentMethod = RequiredText(Get("PaymentMethod"), "PaymentMethod"),
            Notes = EmptyToNull(Get("Notes"))
        };
    }

    private static IEnumerable<ImportMappingInput> NormalizeMappings(IEnumerable<ImportMappingInput> mappings)
    {
        return mappings
            .Where(mapping => !string.IsNullOrWhiteSpace(mapping.SourceColumn) && !string.IsNullOrWhiteSpace(mapping.TargetField))
            .Select(mapping => new ImportMappingInput
            {
                SourceColumn = mapping.SourceColumn.Trim(),
                TargetField = NormalizeTargetField(mapping.TargetField),
                IsRequired = mapping.IsRequired
            })
            .Where(mapping => AllTargetFields.Contains(mapping.TargetField, StringComparer.OrdinalIgnoreCase));
    }

    private static string NormalizeTargetField(string targetField)
    {
        return AllTargetFields.FirstOrDefault(field => string.Equals(field, targetField.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? targetField.Trim();
    }

    private Company? ResolveCompany(string companyIdentifier)
    {
        var parsedCompanyId = int.TryParse(companyIdentifier, NumberStyles.Integer, CultureInfo.InvariantCulture, out var companyId)
            ? companyId
            : (int?)null;

        return _context.Companies.AsNoTracking().FirstOrDefault(company =>
            (parsedCompanyId.HasValue && company.CompanyId == parsedCompanyId.Value)
            || company.LegalName == companyIdentifier
            || company.FEIN == companyIdentifier);
    }

    private async Task<Company?> ResolveCompanyAsync(string companyIdentifier, CancellationToken cancellationToken)
    {
        var parsedCompanyId = int.TryParse(companyIdentifier, NumberStyles.Integer, CultureInfo.InvariantCulture, out var companyId)
            ? companyId
            : (int?)null;

        return await _context.Companies.AsNoTracking().FirstOrDefaultAsync(company =>
            (parsedCompanyId.HasValue && company.CompanyId == parsedCompanyId.Value)
            || company.LegalName == companyIdentifier
            || company.FEIN == companyIdentifier, cancellationToken);
    }

    private static string GetMappedValue(List<ImportMapping> mappings, Dictionary<string, string> rowValues, string targetField)
    {
        var mapping = mappings.FirstOrDefault(candidate => string.Equals(candidate.TargetField, targetField, StringComparison.OrdinalIgnoreCase));
        return mapping != null && rowValues.TryGetValue(mapping.SourceColumn, out var value) ? value.Trim() : string.Empty;
    }

    private static string RequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static DateTime RequiredDate(string value, string fieldName)
    {
        if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
        {
            throw new ValidationException($"{fieldName} requires a valid date.");
        }

        return date.Date;
    }

    private static decimal RequiredMoney(string value, string fieldName)
    {
        if (!decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out var amount))
        {
            throw new ValidationException($"{fieldName} requires a valid amount.");
        }

        return amount;
    }

    private static string? EmptyToNull(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
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

    private static ImportError CreateRowError(int importBatchId, ImportRow row, string columnName, string errorCode, string message)
    {
        return new ImportError
        {
            ImportBatchId = importBatchId,
            ImportRowId = row.ImportRowId,
            RowNumber = row.RowNumber,
            ColumnName = columnName,
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

    private sealed class TaxDepositImportRow
    {
        public string CompanyIdentifier { get; set; } = string.Empty;
        public DateTime DepositDate { get; set; }
        public DateTime TaxPeriodStart { get; set; }
        public DateTime TaxPeriodEnd { get; set; }
        public string TaxType { get; set; } = string.Empty;
        public string Agency { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? ConfirmationNumber { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}

public class TaxDepositsImportValidationResult
{
    public ImportBatch Batch { get; set; } = new();

    public bool CanImport { get; set; }
}

public class TaxDepositsImportConfirmationResult
{
    public ImportBatch Batch { get; set; } = new();

    public int ImportedDepositCount { get; set; }
}
