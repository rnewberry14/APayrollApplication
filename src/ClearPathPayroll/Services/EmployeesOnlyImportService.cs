using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;

namespace ClearPathPayroll.Services;

public class EmployeesOnlyImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static readonly string[] RequiredTargetFields =
    {
        "PayType"
    };

    public static readonly string[] OptionalTargetFields =
    {
        "FullName",
        "FirstName",
        "LastName",
        "EmployeeNumber",
        "SSNLast4",
        "MiddleInitial",
        "Address1",
        "Address2",
        "City",
        "State",
        "ZipCode",
        "Email",
        "Phone",
        "HireDate",
        "EmploymentStatus",
        "HourlyRate",
        "AnnualSalary",
        "FederalFilingStatus",
        "ExtraWithholding",
        "StateTaxState",
        "StateFilingStatus",
        "DirectDepositLast4"
    };

    public static readonly string[] AllTargetFields = RequiredTargetFields
        .Concat(OptionalTargetFields)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private readonly PayrollDbContext _context;
    private readonly ImportService _importService;

    public EmployeesOnlyImportService(PayrollDbContext context, ImportService importService)
    {
        _context = context;
        _importService = importService;
    }

    public Task<ImportFileParseResult> ParseFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        return _importService.ParseFileAsync(stream, fileName, cancellationToken);
    }

    public async Task<EmployeesOnlyImportValidationResult> ValidateAsync(
        int companyId,
        string fileName,
        ImportFileParseResult parseResult,
        IEnumerable<ImportMappingInput> mappings,
        bool demoModeWarningAcknowledged,
        CancellationToken cancellationToken = default)
    {
        if (companyId <= 0)
        {
            throw new ValidationException("Select a company before validating employees.");
        }

        var companyExists = await _context.Companies.AnyAsync(company => company.CompanyId == companyId, cancellationToken);
        if (!companyExists)
        {
            throw new ValidationException("Selected company was not found.");
        }

        var normalizedMappings = NormalizeMappings(mappings).ToList();
        AddRequiredFlags(normalizedMappings);

        var batch = await _importService.CreateStagedBatchAsync(
            fileName,
            ImportType.EmployeesOnly,
            parseResult,
            normalizedMappings,
            demoModeWarningAcknowledged,
            cancellationToken: cancellationToken);

        var result = await ValidateStagedBatchAsync(companyId, batch.ImportBatchId, cancellationToken);
        return result;
    }

    public async Task<EmployeesOnlyImportConfirmationResult> ConfirmAsync(
        int companyId,
        int importBatchId,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateStagedBatchAsync(companyId, importBatchId, cancellationToken);
        if (!validation.CanImport)
        {
            throw new ValidationException("Employee import has validation errors that block confirmation.");
        }

        var batch = validation.Batch;
        var mappings = batch.Mappings.ToList();
        var employees = new List<Employee>();

        foreach (var row in batch.Rows.OrderBy(row => row.RowNumber))
        {
            var values = DeserializeRow(row.RowDataJson);
            employees.Add(BuildEmployee(companyId, mappings, values));
        }

        _context.Employees.AddRange(employees);
        await _context.SaveChangesAsync(cancellationToken);

        var confirmed = await _importService.ConfirmImportAsync(importBatchId, cancellationToken);
        return new EmployeesOnlyImportConfirmationResult
        {
            Batch = confirmed,
            ImportedEmployeeCount = employees.Count
        };
    }

    private async Task<EmployeesOnlyImportValidationResult> ValidateStagedBatchAsync(
        int companyId,
        int importBatchId,
        CancellationToken cancellationToken)
    {
        var batch = await _importService.GetBatchAsync(importBatchId, cancellationToken)
            ?? throw new ValidationException("Import batch was not found.");

        var errors = new List<ImportError>();
        var mappings = batch.Mappings.ToList();
        var targetFields = new HashSet<string>(mappings.Select(mapping => mapping.TargetField), StringComparer.OrdinalIgnoreCase);
        var hasFirstName = targetFields.Contains("FirstName");
        var hasLastName = targetFields.Contains("LastName");
        var hasFullName = targetFields.Contains("FullName");

        foreach (var required in RequiredTargetFields)
        {
            if (!targetFields.Contains(required))
            {
                errors.Add(CreateBatchError(batch.ImportBatchId, "RequiredMappingMissing", $"Required employee field '{required}' is not mapped."));
            }
        }

        if (!hasFullName && (!hasFirstName || !hasLastName))
        {
            errors.Add(CreateBatchError(batch.ImportBatchId, "EmployeeNameMappingMissing", "Map either FirstName and LastName, or map FullName before importing employees."));
        }

        if (!targetFields.Contains("SSNLast4") && !targetFields.Contains("EmployeeNumber"))
        {
            errors.Add(CreateBatchError(batch.ImportBatchId, "IdentityMappingMissing", "Map either SSNLast4 or EmployeeNumber before importing employees."));
        }

        var existingEmployees = await _context.Employees
            .AsNoTracking()
            .Where(employee => employee.CompanyId == companyId)
            .Select(employee => new ExistingEmployeeIdentity(
                employee.EmployeeNumber,
                employee.FirstName,
                employee.LastName,
                employee.SSNLast4))
            .ToListAsync(cancellationToken);

        var fileEmployeeNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var fileNameSsnKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in batch.Rows.OrderBy(row => row.RowNumber))
        {
            var rowValues = DeserializeRow(row.RowDataJson);
            var rowErrors = ValidateRow(batch.ImportBatchId, row, mappings, rowValues, existingEmployees, fileEmployeeNumbers, fileNameSsnKeys);
            errors.AddRange(rowErrors);
            row.Status = rowErrors.Count == 0 ? ImportRowStatus.Valid : ImportRowStatus.Error;
            row.ErrorSummary = rowErrors.Count == 0 ? null : "Employee row has validation errors.";
        }

        if (errors.Count > 0)
        {
            batch.Status = ImportBatchStatus.ValidationFailed;
        }
        else
        {
            batch.Status = ImportBatchStatus.ReadyForConfirmation;
        }

        batch.ValidRowCount = batch.Rows.Count(row => row.Status == ImportRowStatus.Valid);
        batch.ErrorCount = errors.Count;
        batch.UpdatedAt = DateTime.UtcNow;

        _context.ImportErrors.RemoveRange(batch.Errors);
        if (errors.Count > 0)
        {
            _context.ImportErrors.AddRange(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var refreshed = await _importService.GetBatchAsync(importBatchId, cancellationToken)
            ?? throw new ValidationException("Import batch was not found after validation.");

        return new EmployeesOnlyImportValidationResult
        {
            Batch = refreshed,
            CanImport = refreshed.ErrorCount == 0 && refreshed.Status == ImportBatchStatus.ReadyForConfirmation
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

    private static void AddRequiredFlags(List<ImportMappingInput> mappings)
    {
        foreach (var mapping in mappings)
        {
            if (RequiredTargetFields.Contains(mapping.TargetField, StringComparer.OrdinalIgnoreCase))
            {
                mapping.IsRequired = true;
            }
        }
    }

    private static string NormalizeTargetField(string targetField)
    {
        var value = targetField.Trim();
        if (value.StartsWith("Employee.", StringComparison.OrdinalIgnoreCase))
        {
            value = value["Employee.".Length..];
        }

        return AllTargetFields.FirstOrDefault(field => string.Equals(field, value, StringComparison.OrdinalIgnoreCase)) ?? value;
    }

    private static List<ImportError> ValidateRow(
        int importBatchId,
        ImportRow row,
        List<ImportMapping> mappings,
        Dictionary<string, string> rowValues,
        List<ExistingEmployeeIdentity> existingEmployees,
        HashSet<string> fileEmployeeNumbers,
        HashSet<string> fileNameSsnKeys)
    {
        var errors = new List<ImportError>();
        string Get(string targetField) => GetMappedValue(mappings, rowValues, targetField);

        var firstName = Get("FirstName");
        var lastName = Get("LastName");
        var fullName = Get("FullName");
        var employeeNumber = Get("EmployeeNumber");
        var ssnLast4 = Get("SSNLast4");
        var payTypeText = Get("PayType");
        var name = ResolveEmployeeName(
            firstName,
            lastName,
            fullName,
            HasTargetMapping(mappings, "FirstName") && HasTargetMapping(mappings, "LastName"));

        if (string.IsNullOrWhiteSpace(name.FirstName))
        {
            errors.Add(CreateRowError(importBatchId, row, "FirstName", "RequiredValueMissing", "FirstName is required."));
        }

        if (string.IsNullOrWhiteSpace(name.LastName))
        {
            errors.Add(CreateRowError(importBatchId, row, "LastName", "RequiredValueMissing", "LastName is required."));
        }

        if (name.NeedsReview)
        {
            errors.Add(CreateRowError(importBatchId, row, "FullName", "FullNameNeedsReview", "FullName could not be confidently parsed. Map FirstName and LastName or edit the FullName value before importing."));
        }

        if (string.IsNullOrWhiteSpace(employeeNumber) && string.IsNullOrWhiteSpace(ssnLast4))
        {
            errors.Add(CreateRowError(importBatchId, row, "EmployeeNumber", "EmployeeIdentityMissing", "EmployeeNumber or SSNLast4 is required."));
        }

        if (!string.IsNullOrWhiteSpace(ssnLast4) && !IsFourDigits(ssnLast4))
        {
            errors.Add(CreateRowError(importBatchId, row, "SSNLast4", "InvalidSSNLast4", "SSNLast4 must contain exactly four digits. Full SSNs are not accepted."));
        }

        if (!TryParsePayType(payTypeText, out var payType))
        {
            errors.Add(CreateRowError(importBatchId, row, "PayType", "InvalidPayType", "PayType must be Hourly or Salary."));
        }
        else if (payType == PayType.Hourly && !TryParseMoney(Get("HourlyRate"), out var hourlyRate, required: true))
        {
            errors.Add(CreateRowError(importBatchId, row, "HourlyRate", "HourlyRateRequired", "Hourly employees require an hourly rate greater than zero."));
        }
        else if (payType == PayType.Salary && !TryParseMoney(Get("AnnualSalary"), out var annualSalary, required: true))
        {
            errors.Add(CreateRowError(importBatchId, row, "AnnualSalary", "AnnualSalaryRequired", "Salary employees require an annual salary greater than zero."));
        }

        if (!string.IsNullOrWhiteSpace(Get("DirectDepositLast4")) && !IsFourDigits(Get("DirectDepositLast4")))
        {
            errors.Add(CreateRowError(importBatchId, row, "DirectDepositLast4", "InvalidDirectDepositLast4", "DirectDepositLast4 must contain exactly four digits. Full bank account numbers are not accepted."));
        }

        if (LooksLikeSensitiveColumnMapped(mappings))
        {
            errors.Add(CreateRowError(importBatchId, row, "Mapping", "SensitiveColumnMapped", "Full SSN, routing number, and bank account number columns are not accepted."));
        }

        if (!string.IsNullOrWhiteSpace(employeeNumber))
        {
            if (!fileEmployeeNumbers.Add(employeeNumber))
            {
                errors.Add(CreateRowError(importBatchId, row, "EmployeeNumber", "DuplicateEmployeeNumberInFile", "Duplicate EmployeeNumber found in the import file."));
            }

            if (existingEmployees.Any(employee => string.Equals(employee.EmployeeNumber, employeeNumber, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add(CreateRowError(importBatchId, row, "EmployeeNumber", "DuplicateEmployeeNumber", "EmployeeNumber already exists for the selected company."));
            }
        }

        if (!string.IsNullOrWhiteSpace(name.FirstName) && !string.IsNullOrWhiteSpace(name.LastName) && !string.IsNullOrWhiteSpace(ssnLast4))
        {
            var key = $"{name.FirstName}|{name.LastName}|{ssnLast4}";
            if (!fileNameSsnKeys.Add(key))
            {
                errors.Add(CreateRowError(importBatchId, row, "SSNLast4", "DuplicateNameSsnInFile", "Duplicate employee name and SSNLast4 found in the import file."));
            }

            if (existingEmployees.Any(employee =>
                    string.Equals(employee.FirstName, name.FirstName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(employee.LastName, name.LastName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(employee.SSNLast4, ssnLast4, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add(CreateRowError(importBatchId, row, "SSNLast4", "DuplicateNameSsn", "Employee name and SSNLast4 already exist for the selected company."));
            }
        }

        return errors;
    }

    private static Employee BuildEmployee(int companyId, List<ImportMapping> mappings, Dictionary<string, string> rowValues)
    {
        string Get(string targetField) => GetMappedValue(mappings, rowValues, targetField);
        TryParsePayType(Get("PayType"), out var payType);
        TryParseMoney(Get("HourlyRate"), out var hourlyRate, required: false);
        TryParseMoney(Get("AnnualSalary"), out var annualSalary, required: false);
        var name = ResolveEmployeeName(
            Get("FirstName"),
            Get("LastName"),
            Get("FullName"),
            HasTargetMapping(mappings, "FirstName") && HasTargetMapping(mappings, "LastName"));

        var address1 = EmptyToNull(Get("Address1"));
        var city = EmptyToNull(Get("City")) ?? "Imported";
        var state = EmptyToNull(Get("State"))?.ToUpperInvariant() ?? "NA";
        var zipCode = EmptyToNull(Get("ZipCode")) ?? "00000";

        return new Employee
        {
            CompanyId = companyId,
            FirstName = name.FirstName.Trim(),
            MiddleInitial = Truncate(EmptyToNull(Get("MiddleInitial")) ?? name.MiddleInitial, 1),
            LastName = name.LastName.Trim(),
            SSNLast4 = string.IsNullOrWhiteSpace(Get("SSNLast4")) ? "0000" : Get("SSNLast4").Trim(),
            EmployeeNumber = EmptyToNull(Get("EmployeeNumber")),
            DateOfBirth = new DateTime(1900, 1, 1),
            HireDate = TryParseDate(Get("HireDate"), out var hireDate) ? hireDate : DateTime.Today,
            EmploymentStatus = TryParseEmploymentStatus(Get("EmploymentStatus"), out var employmentStatus) ? employmentStatus : EmploymentStatus.Active,
            WorkerType = WorkerType.W2Employee,
            PayType = payType,
            HourlyRate = payType == PayType.Hourly ? hourlyRate : null,
            AnnualSalary = payType == PayType.Salary ? annualSalary : null,
            ResidenceAddress = address1 ?? "Imported address not provided",
            Address1 = address1,
            Address2 = EmptyToNull(Get("Address2")),
            City = city,
            State = Truncate(state, 2) ?? "NA",
            ZipCode = zipCode,
            Email = EmptyToNull(Get("Email")),
            Phone = EmptyToNull(Get("Phone")),
            FilingStatus = TryParseFederalFilingStatus(Get("FederalFilingStatus"), out var filingStatus) ? filingStatus : FederalFilingStatus.NotSpecified,
            ExtraWithholding = TryParseMoney(Get("ExtraWithholding"), out var extraWithholding, required: false) ? extraWithholding : 0m,
            StateTaxState = Truncate(EmptyToNull(Get("StateTaxState"))?.ToUpperInvariant(), 2),
            StateFilingStatus = TryParseStateFilingStatus(Get("StateFilingStatus"), out var stateFilingStatus) ? stateFilingStatus : StateFilingStatus.NotSpecified,
            IsActive = true,
            MinimumWageWarningEnabled = true,
            CreatedAt = DateTime.UtcNow,
            PayrollProfileCreatedAt = DateTime.UtcNow
        };
    }

    public static EmployeeFullNameParseResult ParseFullName(string fullName)
    {
        var value = fullName.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            return EmployeeFullNameParseResult.NeedsReviewResult();
        }

        if (value.Contains(',', StringComparison.Ordinal))
        {
            var commaParts = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (commaParts.Length == 2)
            {
                var firstParts = commaParts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (firstParts.Length >= 1 && LooksLikeNameToken(firstParts[0]) && LooksLikeNameToken(commaParts[0]))
                {
                    return new EmployeeFullNameParseResult(
                        firstParts[0],
                        firstParts.Length > 1 ? firstParts[1][..1] : null,
                        commaParts[0],
                        NeedsReview: false);
                }
            }

            return EmployeeFullNameParseResult.NeedsReviewResult();
        }

        var parts = value.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 && parts.All(LooksLikeNameToken))
        {
            return new EmployeeFullNameParseResult(parts[0], null, parts[1], NeedsReview: false);
        }

        if (parts.Length == 3 && parts.All(LooksLikeNameToken))
        {
            return new EmployeeFullNameParseResult(parts[0], parts[1][..1], parts[2], NeedsReview: false);
        }

        return EmployeeFullNameParseResult.NeedsReviewResult();
    }

    private static EmployeeFullNameParseResult ResolveEmployeeName(string firstName, string lastName, string fullName, bool preferSeparateNameFields)
    {
        if (preferSeparateNameFields || string.IsNullOrWhiteSpace(fullName))
        {
            return new EmployeeFullNameParseResult(firstName.Trim(), null, lastName.Trim(), NeedsReview: false);
        }

        return ParseFullName(fullName);
    }

    private static bool HasTargetMapping(IEnumerable<ImportMapping> mappings, string targetField)
    {
        return mappings.Any(mapping => string.Equals(mapping.TargetField, targetField, StringComparison.OrdinalIgnoreCase));
    }

    private static bool LooksLikeNameToken(string value)
    {
        return value.Length > 0
            && value.Any(char.IsLetter)
            && value.All(character => char.IsLetter(character) || character == '\'' || character == '-');
    }

    private static string GetMappedValue(List<ImportMapping> mappings, Dictionary<string, string> rowValues, string targetField)
    {
        var mapping = mappings.FirstOrDefault(candidate => string.Equals(candidate.TargetField, targetField, StringComparison.OrdinalIgnoreCase));
        if (mapping == null)
        {
            return string.Empty;
        }

        return rowValues.TryGetValue(mapping.SourceColumn, out var value) ? value.Trim() : string.Empty;
    }

    private static bool LooksLikeSensitiveColumnMapped(IEnumerable<ImportMapping> mappings)
    {
        return mappings.Any(mapping =>
        {
            var source = mapping.SourceColumn.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
            return string.Equals(source, "SSN", StringComparison.OrdinalIgnoreCase)
                || string.Equals(source, "SocialSecurityNumber", StringComparison.OrdinalIgnoreCase)
                || source.Contains("FullSSN", StringComparison.OrdinalIgnoreCase)
                || source.Contains("RoutingNumber", StringComparison.OrdinalIgnoreCase)
                || source.Contains("AccountNumber", StringComparison.OrdinalIgnoreCase)
                || source.Contains("BankAccount", StringComparison.OrdinalIgnoreCase);
        });
    }

    private static bool TryParsePayType(string value, out PayType payType)
    {
        if (Enum.TryParse(value, ignoreCase: true, out payType))
        {
            return true;
        }

        payType = PayType.Hourly;
        return false;
    }

    private static bool TryParseEmploymentStatus(string value, out EmploymentStatus status)
    {
        return Enum.TryParse(value, ignoreCase: true, out status);
    }

    private static bool TryParseFederalFilingStatus(string value, out FederalFilingStatus status)
    {
        return Enum.TryParse(value, ignoreCase: true, out status);
    }

    private static bool TryParseStateFilingStatus(string value, out StateFilingStatus status)
    {
        return Enum.TryParse(value, ignoreCase: true, out status);
    }

    private static bool TryParseMoney(string value, out decimal amount, bool required)
    {
        if (decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out amount) && (!required || amount > 0))
        {
            return true;
        }

        amount = 0m;
        return !required && string.IsNullOrWhiteSpace(value);
    }

    private static bool TryParseDate(string value, out DateTime date)
    {
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date);
    }

    private static bool IsFourDigits(string value)
    {
        return value.Length == 4 && value.All(char.IsDigit);
    }

    private static string? EmptyToNull(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? Truncate(string? value, int length)
    {
        return value == null || value.Length <= length ? value : value[..length];
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

    private static ImportError CreateRowError(ImportBatch batch, ImportRow row, string columnName, string errorCode, string message)
    {
        return CreateRowError(batch.ImportBatchId, row, columnName, errorCode, message);
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

    private sealed record ExistingEmployeeIdentity(string? EmployeeNumber, string FirstName, string LastName, string SSNLast4);
}

public sealed record EmployeeFullNameParseResult(string FirstName, string? MiddleInitial, string LastName, bool NeedsReview)
{
    public static EmployeeFullNameParseResult NeedsReviewResult()
    {
        return new EmployeeFullNameParseResult(string.Empty, null, string.Empty, NeedsReview: true);
    }
}

public class EmployeesOnlyImportValidationResult
{
    public ImportBatch Batch { get; set; } = new();

    public bool CanImport { get; set; }
}

public class EmployeesOnlyImportConfirmationResult
{
    public ImportBatch Batch { get; set; } = new();

    public int ImportedEmployeeCount { get; set; }
}
