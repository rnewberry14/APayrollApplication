using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;

namespace ClearPathPayroll.Services;

public class W2ImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static readonly string[] RequiredTargetFields =
    {
        "TaxYear",
        "EmployerName",
        "EmployerEIN",
        "EmployerAddress",
        "EmployeeFirstName",
        "EmployeeLastName",
        "EmployeeSSNLast4",
        "EmployeeAddress",
        "Box1Wages",
        "Box2FederalTaxWithheld",
        "Box3SocialSecurityWages",
        "Box4SocialSecurityTaxWithheld",
        "Box5MedicareWages",
        "Box6MedicareTaxWithheld"
    };

    public static readonly string[] OptionalTargetFields =
    {
        "Box12CodeAndAmount",
        "Box14DescriptionAndAmount",
        "StateWages",
        "StateTaxWithheld",
        "LocalWages",
        "LocalTaxWithheld"
    };

    public static readonly string[] AllTargetFields = RequiredTargetFields.Concat(OptionalTargetFields).ToArray();

    private readonly PayrollDbContext _context;
    private readonly ImportService _importService;

    public W2ImportService(PayrollDbContext context, ImportService importService)
    {
        _context = context;
        _importService = importService;
    }

    public Task<ImportFileParseResult> ParseFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        return _importService.ParseFileAsync(stream, fileName, cancellationToken);
    }

    public async Task<W2ImportValidationResult> ValidateAsync(
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
            ImportType.W2Historical,
            parseResult,
            normalizedMappings,
            reviewAcknowledged,
            cancellationToken: cancellationToken);

        return await ValidateStagedBatchAsync(batch.ImportBatchId, cancellationToken);
    }

    public async Task<W2ImportConfirmationResult> ConfirmAsync(int importBatchId, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateStagedBatchAsync(importBatchId, cancellationToken);
        if (!validation.CanImport)
        {
            throw new ValidationException("W-2 import has validation errors that block confirmation.");
        }

        var batch = validation.Batch;
        var records = new List<W2HistoricalRecord>();

        foreach (var row in batch.Rows.OrderBy(row => row.RowNumber))
        {
            var parsed = ParseW2Row(batch.Mappings.ToList(), DeserializeRow(row.RowDataJson));
            records.Add(new W2HistoricalRecord
            {
                TaxYear = parsed.TaxYear,
                EmployerName = parsed.EmployerName,
                EmployerEINMasked = MaskEin(parsed.EmployerEIN),
                EmployerAddress = parsed.EmployerAddress,
                EmployeeFirstName = parsed.EmployeeFirstName,
                EmployeeLastName = parsed.EmployeeLastName,
                EmployeeSSNLast4 = parsed.EmployeeSSNLast4,
                EmployeeAddress = parsed.EmployeeAddress,
                Box1Wages = parsed.Box1Wages,
                Box2FederalTaxWithheld = parsed.Box2FederalTaxWithheld,
                Box3SocialSecurityWages = parsed.Box3SocialSecurityWages,
                Box4SocialSecurityTaxWithheld = parsed.Box4SocialSecurityTaxWithheld,
                Box5MedicareWages = parsed.Box5MedicareWages,
                Box6MedicareTaxWithheld = parsed.Box6MedicareTaxWithheld,
                Box12CodeAndAmountPlaceholders = parsed.Box12CodeAndAmount,
                Box14DescriptionAndAmountPlaceholders = parsed.Box14DescriptionAndAmount,
                StateWages = parsed.StateWages,
                StateTaxWithheld = parsed.StateTaxWithheld,
                LocalWages = parsed.LocalWages,
                LocalTaxWithheld = parsed.LocalTaxWithheld,
                RecordSource = "historical user-entered data",
                ImportBatchId = batch.ImportBatchId,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.W2HistoricalRecords.AddRange(records);
        await _context.SaveChangesAsync(cancellationToken);

        var confirmed = await _importService.ConfirmImportAsync(importBatchId, cancellationToken);
        return new W2ImportConfirmationResult
        {
            Batch = confirmed,
            ImportedRecordCount = records.Count
        };
    }

    private async Task<W2ImportValidationResult> ValidateStagedBatchAsync(int importBatchId, CancellationToken cancellationToken)
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
                errors.Add(CreateBatchError(batch.ImportBatchId, "RequiredMappingMissing", $"Required W-2 field '{required}' is not mapped."));
            }
        }

        foreach (var row in batch.Rows.OrderBy(row => row.RowNumber))
        {
            var rowErrors = ValidateRow(batch.ImportBatchId, row, mappings, DeserializeRow(row.RowDataJson));
            errors.AddRange(rowErrors);
            row.Status = rowErrors.Count == 0 ? ImportRowStatus.Valid : ImportRowStatus.Error;
            row.ErrorSummary = rowErrors.Count == 0 ? null : "W-2 row has validation errors.";
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

        return new W2ImportValidationResult
        {
            Batch = refreshed,
            CanImport = refreshed.Status == ImportBatchStatus.ReadyForConfirmation && refreshed.ErrorCount == 0
        };
    }

    private static List<ImportError> ValidateRow(int importBatchId, ImportRow row, List<ImportMapping> mappings, Dictionary<string, string> values)
    {
        var errors = new List<ImportError>();
        W2ImportRow? parsed = null;

        try
        {
            parsed = ParseW2Row(mappings, values);
        }
        catch (ValidationException ex)
        {
            errors.Add(CreateRowError(importBatchId, row, "Row", "InvalidW2Row", ex.Message));
        }

        if (parsed == null)
        {
            return errors;
        }

        if (parsed.TaxYear < 1900 || parsed.TaxYear > DateTime.Today.Year + 1)
        {
            errors.Add(CreateRowError(importBatchId, row, "TaxYear", "InvalidTaxYear", "TaxYear is outside the accepted range."));
        }

        if (!IsFourDigits(parsed.EmployeeSSNLast4))
        {
            errors.Add(CreateRowError(importBatchId, row, "EmployeeSSNLast4", "InvalidEmployeeSSNLast4", "EmployeeSSNLast4 requires exactly four digits. Full SSNs are not accepted."));
        }

        if (LooksLikeFullSsnColumn(mappings))
        {
            errors.Add(CreateRowError(importBatchId, row, "Mapping", "FullSsnColumnMapped", "Full SSN columns are not accepted for W-2 import."));
        }

        if (new[]
            {
                parsed.Box1Wages,
                parsed.Box2FederalTaxWithheld,
                parsed.Box3SocialSecurityWages,
                parsed.Box4SocialSecurityTaxWithheld,
                parsed.Box5MedicareWages,
                parsed.Box6MedicareTaxWithheld,
                parsed.StateWages ?? 0m,
                parsed.StateTaxWithheld ?? 0m,
                parsed.LocalWages ?? 0m,
                parsed.LocalTaxWithheld ?? 0m
            }.Any(amount => amount < 0))
        {
            errors.Add(CreateRowError(importBatchId, row, "Amount", "NegativeAmount", "W-2 wage and tax amounts cannot be negative."));
        }

        return errors;
    }

    private static W2ImportRow ParseW2Row(List<ImportMapping> mappings, Dictionary<string, string> values)
    {
        string Get(string targetField) => GetMappedValue(mappings, values, targetField);

        return new W2ImportRow
        {
            TaxYear = RequiredInt(Get("TaxYear"), "TaxYear"),
            EmployerName = RequiredText(Get("EmployerName"), "EmployerName"),
            EmployerEIN = RequiredText(Get("EmployerEIN"), "EmployerEIN"),
            EmployerAddress = RequiredText(Get("EmployerAddress"), "EmployerAddress"),
            EmployeeFirstName = RequiredText(Get("EmployeeFirstName"), "EmployeeFirstName"),
            EmployeeLastName = RequiredText(Get("EmployeeLastName"), "EmployeeLastName"),
            EmployeeSSNLast4 = RequiredText(Get("EmployeeSSNLast4"), "EmployeeSSNLast4"),
            EmployeeAddress = RequiredText(Get("EmployeeAddress"), "EmployeeAddress"),
            Box1Wages = RequiredMoney(Get("Box1Wages"), "Box1Wages"),
            Box2FederalTaxWithheld = RequiredMoney(Get("Box2FederalTaxWithheld"), "Box2FederalTaxWithheld"),
            Box3SocialSecurityWages = RequiredMoney(Get("Box3SocialSecurityWages"), "Box3SocialSecurityWages"),
            Box4SocialSecurityTaxWithheld = RequiredMoney(Get("Box4SocialSecurityTaxWithheld"), "Box4SocialSecurityTaxWithheld"),
            Box5MedicareWages = RequiredMoney(Get("Box5MedicareWages"), "Box5MedicareWages"),
            Box6MedicareTaxWithheld = RequiredMoney(Get("Box6MedicareTaxWithheld"), "Box6MedicareTaxWithheld"),
            Box12CodeAndAmount = EmptyToNull(Get("Box12CodeAndAmount")),
            Box14DescriptionAndAmount = EmptyToNull(Get("Box14DescriptionAndAmount")),
            StateWages = OptionalMoney(Get("StateWages")),
            StateTaxWithheld = OptionalMoney(Get("StateTaxWithheld")),
            LocalWages = OptionalMoney(Get("LocalWages")),
            LocalTaxWithheld = OptionalMoney(Get("LocalTaxWithheld"))
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

    public static string MaskEin(string employerEin)
    {
        var digits = new string(employerEin.Where(char.IsDigit).ToArray());
        if (digits.Length >= 4)
        {
            return $"**-***{digits[^4..]}";
        }

        return string.IsNullOrWhiteSpace(employerEin) ? string.Empty : "masked";
    }

    private static bool LooksLikeFullSsnColumn(IEnumerable<ImportMapping> mappings)
    {
        return mappings.Any(mapping =>
        {
            var source = mapping.SourceColumn.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
            return string.Equals(source, "SSN", StringComparison.OrdinalIgnoreCase)
                || source.Contains("FullSSN", StringComparison.OrdinalIgnoreCase)
                || source.Contains("SocialSecurityNumber", StringComparison.OrdinalIgnoreCase);
        });
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

    private static int RequiredInt(string value, string fieldName)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            throw new ValidationException($"{fieldName} requires a valid whole number.");
        }

        return result;
    }

    private static decimal RequiredMoney(string value, string fieldName)
    {
        if (!decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out var amount))
        {
            throw new ValidationException($"{fieldName} requires a valid amount.");
        }

        return amount;
    }

    private static decimal? OptionalMoney(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out var amount))
        {
            throw new ValidationException("Optional W-2 amount fields require valid amounts when provided.");
        }

        return amount;
    }

    private static string? EmptyToNull(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static bool IsFourDigits(string value)
    {
        return value.Length == 4 && value.All(char.IsDigit);
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

    private sealed class W2ImportRow
    {
        public int TaxYear { get; set; }
        public string EmployerName { get; set; } = string.Empty;
        public string EmployerEIN { get; set; } = string.Empty;
        public string EmployerAddress { get; set; } = string.Empty;
        public string EmployeeFirstName { get; set; } = string.Empty;
        public string EmployeeLastName { get; set; } = string.Empty;
        public string EmployeeSSNLast4 { get; set; } = string.Empty;
        public string EmployeeAddress { get; set; } = string.Empty;
        public decimal Box1Wages { get; set; }
        public decimal Box2FederalTaxWithheld { get; set; }
        public decimal Box3SocialSecurityWages { get; set; }
        public decimal Box4SocialSecurityTaxWithheld { get; set; }
        public decimal Box5MedicareWages { get; set; }
        public decimal Box6MedicareTaxWithheld { get; set; }
        public string? Box12CodeAndAmount { get; set; }
        public string? Box14DescriptionAndAmount { get; set; }
        public decimal? StateWages { get; set; }
        public decimal? StateTaxWithheld { get; set; }
        public decimal? LocalWages { get; set; }
        public decimal? LocalTaxWithheld { get; set; }
    }
}

public class W2ImportValidationResult
{
    public ImportBatch Batch { get; set; } = new();

    public bool CanImport { get; set; }
}

public class W2ImportConfirmationResult
{
    public ImportBatch Batch { get; set; } = new();

    public int ImportedRecordCount { get; set; }
}
