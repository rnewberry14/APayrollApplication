using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;

namespace ClearPathPayroll.Services;

public class ChecksImportService
{
    private const decimal RoundingTolerance = 0.01m;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static readonly string[] RequiredTargetFields =
    {
        "CompanyIdentifier",
        "EmployeeIdentifier",
        "PayDate",
        "PayPeriodStart",
        "PayPeriodEnd",
        "GrossPay",
        "RegularHours",
        "OvertimeHours",
        "EmployeeFederalTax",
        "EmployeeStateTax",
        "EmployeeLocalTax",
        "SocialSecurityTax",
        "MedicareTax",
        "Deductions",
        "NetPay",
        "PaymentMethod"
    };

    public static readonly string[] OptionalTargetFields =
    {
        "CheckNumber",
        "DirectDepositLast4"
    };

    public static readonly string[] AllTargetFields = RequiredTargetFields
        .Concat(OptionalTargetFields)
        .ToArray();

    private readonly PayrollDbContext _context;
    private readonly ImportService _importService;

    public ChecksImportService(PayrollDbContext context, ImportService importService)
    {
        _context = context;
        _importService = importService;
    }

    public Task<ImportFileParseResult> ParseFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        return _importService.ParseFileAsync(stream, fileName, cancellationToken);
    }

    public async Task<ChecksImportValidationResult> ValidateAsync(
        ChecksImportMode mode,
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
            mode == ChecksImportMode.ChecksOnly ? ImportType.ChecksOnly : ImportType.EmployeesAndChecksOnly,
            parseResult,
            normalizedMappings,
            reviewAcknowledged,
            cancellationToken: cancellationToken);

        return await ValidateStagedBatchAsync(mode, batch.ImportBatchId, cancellationToken);
    }

    public async Task<ChecksImportConfirmationResult> ConfirmAsync(
        ChecksImportMode mode,
        int importBatchId,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateStagedBatchAsync(mode, importBatchId, cancellationToken);
        if (!validation.CanImport)
        {
            throw new ValidationException("Imported checks have validation errors that block saving.");
        }

        var batch = validation.Batch;
        var rows = batch.Rows
            .OrderBy(row => row.RowNumber)
            .ToList();
        var parsedRows = new List<CheckImportRow>();

        foreach (var row in rows)
        {
            var parsed = ParseCheckRow(batch.Mappings.ToList(), DeserializeRow(row.RowDataJson));
            var company = await ResolveCompanyAsync(parsed.CompanyIdentifier, cancellationToken)
                ?? throw new ValidationException("CompanyIdentifier did not match a local company.");
            parsed.CompanyId = company.CompanyId;
            parsedRows.Add(parsed);
        }

        var createdRuns = new List<PayrollRun>();
        foreach (var group in parsedRows.GroupBy(row => new { row.CompanyId, row.PayDate, row.PayPeriodStart, row.PayPeriodEnd }))
        {
            var paySchedule = await GetOrCreateImportPayScheduleAsync(
                group.Key.CompanyId,
                group.Key.PayDate,
                group.Key.PayPeriodStart,
                group.Key.PayPeriodEnd,
                cancellationToken);

            var payrollRun = new PayrollRun
            {
                CompanyId = group.Key.CompanyId,
                PayScheduleId = paySchedule.PayScheduleId,
                PayPeriodStart = group.Key.PayPeriodStart,
                PayPeriodEnd = group.Key.PayPeriodEnd,
                PayDate = group.Key.PayDate,
                Status = PayrollStatus.Draft,
                PayrollMode = "After-the-fact payroll",
                CreatedByUserId = "local-import-user",
                CreatedAt = DateTime.UtcNow
            };

            foreach (var row in group)
            {
                var employee = await ResolveEmployeeForConfirmationAsync(mode, row, cancellationToken);
                var employeeTaxes = row.EmployeeFederalTax
                    + row.EmployeeStateTax
                    + row.EmployeeLocalTax
                    + row.SocialSecurityTax
                    + row.MedicareTax;

                var payrollEmployee = new PayrollRunEmployee
                {
                    EmployeeId = employee.EmployeeId,
                    GrossPay = row.GrossPay,
                    TotalTaxes = employeeTaxes,
                    TotalDeductions = row.Deductions,
                    NetPay = row.NetPay
                };

                payrollEmployee.EarningLines.Add(new EarningLine
                {
                    Description = "Imported regular earnings",
                    Hours = row.RegularHours,
                    Amount = row.GrossPay
                });

                if (row.OvertimeHours > 0)
                {
                    payrollEmployee.EarningLines.Add(new EarningLine
                    {
                        Description = "Imported overtime hours placeholder",
                        Hours = row.OvertimeHours,
                        Amount = 0m
                    });
                }

                if (row.Deductions > 0)
                {
                    payrollEmployee.DeductionLines.Add(new DeductionLine
                    {
                        Description = "Imported deductions",
                        Amount = row.Deductions
                    });
                }

                AddTaxLine(payrollEmployee, "Employee federal tax", row.EmployeeFederalTax);
                AddTaxLine(payrollEmployee, "Employee state tax", row.EmployeeStateTax);
                AddTaxLine(payrollEmployee, "Employee local tax", row.EmployeeLocalTax);
                AddTaxLine(payrollEmployee, "Social Security tax", row.SocialSecurityTax);
                AddTaxLine(payrollEmployee, "Medicare tax", row.MedicareTax);

                payrollEmployee.NetPayLines.Add(new NetPayLine
                {
                    Amount = row.NetPay,
                    PaymentMethod = BuildPaymentMethod(row)
                });

                payrollRun.PayrollRunEmployees.Add(payrollEmployee);
            }

            payrollRun.TotalGrossPay = payrollRun.PayrollRunEmployees.Sum(employee => employee.GrossPay);
            payrollRun.TotalEmployeeTaxes = payrollRun.PayrollRunEmployees.Sum(employee => employee.TotalTaxes);
            payrollRun.TotalDeductions = payrollRun.PayrollRunEmployees.Sum(employee => employee.TotalDeductions);
            payrollRun.TotalNetPay = payrollRun.PayrollRunEmployees.Sum(employee => employee.NetPay);
            createdRuns.Add(payrollRun);
            _context.PayrollRuns.Add(payrollRun);
        }

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var run in createdRuns)
        {
            _context.AuditLogEntries.Add(new AuditLogEntry
            {
                PayrollRunId = run.PayrollRunId,
                EventType = "AfterTheFactPayrollImported",
                Description = $"After-the-fact payroll import created draft payroll run {run.PayrollRunId}.",
                CreatedByUserId = "local-import-user",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        var confirmedBatch = await _importService.ConfirmImportAsync(importBatchId, cancellationToken);

        return new ChecksImportConfirmationResult
        {
            Batch = confirmedBatch,
            PayrollRunCount = createdRuns.Count,
            CheckCount = parsedRows.Count,
            PayrollRunIds = createdRuns.Select(run => run.PayrollRunId).ToList()
        };
    }

    private async Task<ChecksImportValidationResult> ValidateStagedBatchAsync(
        ChecksImportMode mode,
        int importBatchId,
        CancellationToken cancellationToken)
    {
        var batch = await _importService.GetBatchAsync(importBatchId, cancellationToken)
            ?? throw new ValidationException("Import batch was not found.");

        var errors = new List<ImportError>();
        var mappings = batch.Mappings.ToList();
        var mappedTargets = new HashSet<string>(mappings.Select(mapping => mapping.TargetField), StringComparer.OrdinalIgnoreCase);

        foreach (var required in RequiredTargetFields)
        {
            if (!mappedTargets.Contains(required))
            {
                errors.Add(CreateBatchError(batch.ImportBatchId, "RequiredMappingMissing", $"Required check field '{required}' is not mapped."));
            }
        }

        foreach (var row in batch.Rows.OrderBy(row => row.RowNumber))
        {
            var values = DeserializeRow(row.RowDataJson);
            var rowErrors = ValidateRow(mode, batch.ImportBatchId, row, mappings, values);
            errors.AddRange(rowErrors);
            row.Status = rowErrors.Count == 0 ? ImportRowStatus.Valid : ImportRowStatus.Error;
            row.ErrorSummary = rowErrors.Count == 0 ? null : "Imported check row has validation errors.";
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

        return new ChecksImportValidationResult
        {
            Batch = refreshed,
            CanImport = refreshed.Status == ImportBatchStatus.ReadyForConfirmation && refreshed.ErrorCount == 0
        };
    }

    private List<ImportError> ValidateRow(
        ChecksImportMode mode,
        int importBatchId,
        ImportRow row,
        List<ImportMapping> mappings,
        Dictionary<string, string> values)
    {
        var errors = new List<ImportError>();
        CheckImportRow? parsed = null;

        try
        {
            parsed = ParseCheckRow(mappings, values);
        }
        catch (ValidationException ex)
        {
            errors.Add(CreateRowError(importBatchId, row, "Row", "InvalidCheckRow", ex.Message));
        }

        if (parsed == null)
        {
            return errors;
        }

        var expectedNet = parsed.GrossPay
            - parsed.EmployeeFederalTax
            - parsed.EmployeeStateTax
            - parsed.EmployeeLocalTax
            - parsed.SocialSecurityTax
            - parsed.MedicareTax
            - parsed.Deductions;

        if (Math.Abs(expectedNet - parsed.NetPay) > RoundingTolerance)
        {
            errors.Add(CreateRowError(importBatchId, row, "NetPay", "NetPayMismatch", "Gross pay minus employee taxes and deductions does not match NetPay within rounding tolerance."));
        }

        if (!string.IsNullOrWhiteSpace(parsed.DirectDepositLast4) && !IsFourDigits(parsed.DirectDepositLast4))
        {
            errors.Add(CreateRowError(importBatchId, row, "DirectDepositLast4", "InvalidDirectDepositLast4", "DirectDepositLast4 requires exactly four digits."));
        }

        if (LooksLikeSensitiveColumnMapped(mappings))
        {
            errors.Add(CreateRowError(importBatchId, row, "Mapping", "SensitiveColumnMapped", "Full SSN, routing number, and bank account number columns are not accepted."));
        }

        var company = ResolveCompany(parsed.CompanyIdentifier);

        if (company == null)
        {
            errors.Add(CreateRowError(importBatchId, row, "CompanyIdentifier", "CompanyNotFound", "CompanyIdentifier did not match a local company."));
        }
        else
        {
            parsed.CompanyId = company.CompanyId;
            var employeeExists = _context.Employees.AsNoTracking().Any(employee =>
                employee.CompanyId == company.CompanyId
                && (employee.EmployeeId.ToString(CultureInfo.InvariantCulture) == parsed.EmployeeIdentifier
                    || employee.EmployeeNumber == parsed.EmployeeIdentifier));

            if (!employeeExists && mode == ChecksImportMode.ChecksOnly)
            {
                errors.Add(CreateRowError(importBatchId, row, "EmployeeIdentifier", "EmployeeNotFound", "EmployeeIdentifier did not match a local employee."));
            }
        }

        return errors;
    }

    private static CheckImportRow ParseCheckRow(List<ImportMapping> mappings, Dictionary<string, string> values)
    {
        string Get(string targetField) => GetMappedValue(mappings, values, targetField);

        return new CheckImportRow
        {
            CompanyIdentifier = RequiredText(Get("CompanyIdentifier"), "CompanyIdentifier"),
            EmployeeIdentifier = RequiredText(Get("EmployeeIdentifier"), "EmployeeIdentifier"),
            PayDate = RequiredDate(Get("PayDate"), "PayDate"),
            PayPeriodStart = RequiredDate(Get("PayPeriodStart"), "PayPeriodStart"),
            PayPeriodEnd = RequiredDate(Get("PayPeriodEnd"), "PayPeriodEnd"),
            GrossPay = RequiredMoney(Get("GrossPay"), "GrossPay"),
            RegularHours = RequiredMoney(Get("RegularHours"), "RegularHours"),
            OvertimeHours = RequiredMoney(Get("OvertimeHours"), "OvertimeHours"),
            EmployeeFederalTax = RequiredMoney(Get("EmployeeFederalTax"), "EmployeeFederalTax"),
            EmployeeStateTax = RequiredMoney(Get("EmployeeStateTax"), "EmployeeStateTax"),
            EmployeeLocalTax = RequiredMoney(Get("EmployeeLocalTax"), "EmployeeLocalTax"),
            SocialSecurityTax = RequiredMoney(Get("SocialSecurityTax"), "SocialSecurityTax"),
            MedicareTax = RequiredMoney(Get("MedicareTax"), "MedicareTax"),
            Deductions = RequiredMoney(Get("Deductions"), "Deductions"),
            NetPay = RequiredMoney(Get("NetPay"), "NetPay"),
            PaymentMethod = RequiredText(Get("PaymentMethod"), "PaymentMethod"),
            CheckNumber = EmptyToNull(Get("CheckNumber")),
            DirectDepositLast4 = EmptyToNull(Get("DirectDepositLast4"))
        };
    }

    private async Task<PaySchedule> GetOrCreateImportPayScheduleAsync(
        int companyId,
        DateTime payDate,
        DateTime periodStart,
        DateTime periodEnd,
        CancellationToken cancellationToken)
    {
        var schedule = await _context.PaySchedules.FirstOrDefaultAsync(schedule =>
            schedule.CompanyId == companyId
            && schedule.Name == "Imported after-the-fact payroll", cancellationToken);

        if (schedule != null)
        {
            return schedule;
        }

        schedule = new PaySchedule
        {
            CompanyId = companyId,
            Name = "Imported after-the-fact payroll",
            Frequency = PayFrequency.Biweekly,
            NextPayDate = payDate,
            NextPeriodStartDate = periodStart,
            NextPeriodEndDate = periodEnd,
            IsActive = true
        };

        _context.PaySchedules.Add(schedule);
        await _context.SaveChangesAsync(cancellationToken);
        return schedule;
    }

    private async Task<Employee> ResolveEmployeeForConfirmationAsync(
        ChecksImportMode mode,
        CheckImportRow row,
        CancellationToken cancellationToken)
    {
        var parsedEmployeeId = int.TryParse(row.EmployeeIdentifier, NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId)
            ? employeeId
            : (int?)null;

        var employee = await _context.Employees.FirstOrDefaultAsync(candidate =>
            candidate.CompanyId == row.CompanyId
            && ((parsedEmployeeId.HasValue && candidate.EmployeeId == parsedEmployeeId.Value)
                || candidate.EmployeeNumber == row.EmployeeIdentifier), cancellationToken);

        if (employee != null)
        {
            return employee;
        }

        if (mode == ChecksImportMode.ChecksOnly)
        {
            throw new ValidationException("EmployeeIdentifier did not match a local employee.");
        }

        employee = new Employee
        {
            CompanyId = row.CompanyId,
            EmployeeNumber = row.EmployeeIdentifier,
            FirstName = "Imported",
            LastName = row.EmployeeIdentifier,
            SSNLast4 = "0000",
            DateOfBirth = new DateTime(1900, 1, 1),
            HireDate = row.PayPeriodStart,
            EmploymentStatus = EmploymentStatus.Active,
            WorkerType = WorkerType.W2Employee,
            PayType = PayType.Hourly,
            HourlyRate = row.RegularHours > 0 ? Math.Round(row.GrossPay / row.RegularHours, 2) : 1m,
            ResidenceAddress = "Imported address not provided",
            City = "Imported",
            State = "NA",
            ZipCode = "00000",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            PayrollProfileCreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
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
        if (!decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out var amount) || amount < 0)
        {
            throw new ValidationException($"{fieldName} requires a non-negative amount.");
        }

        return amount;
    }

    private static void AddTaxLine(PayrollRunEmployee payrollEmployee, string taxType, decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        payrollEmployee.TaxLines.Add(new TaxLine
        {
            TaxType = taxType,
            Amount = amount
        });
    }

    private static string BuildPaymentMethod(CheckImportRow row)
    {
        if (row.PaymentMethod.Contains("direct", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(row.DirectDepositLast4))
        {
            return $"Direct deposit ending {row.DirectDepositLast4}";
        }

        return string.IsNullOrWhiteSpace(row.CheckNumber)
            ? row.PaymentMethod
            : $"{row.PaymentMethod} #{row.CheckNumber}";
    }

    private static bool LooksLikeSensitiveColumnMapped(IEnumerable<ImportMapping> mappings)
    {
        return mappings.Any(mapping =>
        {
            var source = mapping.SourceColumn.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
            return string.Equals(source, "SSN", StringComparison.OrdinalIgnoreCase)
                || source.Contains("FullSSN", StringComparison.OrdinalIgnoreCase)
                || source.Contains("RoutingNumber", StringComparison.OrdinalIgnoreCase)
                || source.Contains("AccountNumber", StringComparison.OrdinalIgnoreCase)
                || source.Contains("BankAccount", StringComparison.OrdinalIgnoreCase);
        });
    }

    private static bool IsFourDigits(string value)
    {
        return value.Length == 4 && value.All(char.IsDigit);
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

    private sealed class CheckImportRow
    {
        public int CompanyId { get; set; }
        public string CompanyIdentifier { get; set; } = string.Empty;
        public string EmployeeIdentifier { get; set; } = string.Empty;
        public DateTime PayDate { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public decimal GrossPay { get; set; }
        public decimal RegularHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal EmployeeFederalTax { get; set; }
        public decimal EmployeeStateTax { get; set; }
        public decimal EmployeeLocalTax { get; set; }
        public decimal SocialSecurityTax { get; set; }
        public decimal MedicareTax { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? CheckNumber { get; set; }
        public string? DirectDepositLast4 { get; set; }
    }
}

public enum ChecksImportMode
{
    ChecksOnly,
    EmployeesAndChecks
}

public class ChecksImportValidationResult
{
    public ImportBatch Batch { get; set; } = new();

    public bool CanImport { get; set; }
}

public class ChecksImportConfirmationResult
{
    public ImportBatch Batch { get; set; } = new();

    public int PayrollRunCount { get; set; }

    public int CheckCount { get; set; }

    public List<int> PayrollRunIds { get; set; } = new();
}
