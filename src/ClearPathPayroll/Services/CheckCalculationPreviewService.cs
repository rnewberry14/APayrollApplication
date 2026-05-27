using ClearPathPayroll.Configuration;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClearPathPayroll.Services;

public sealed class CheckCalculationInput
{
    public int PayrollRunEmployeeId { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal AdditionalEarnings { get; set; }
    public decimal ManualDeductions { get; set; }
    public decimal Reimbursements { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public sealed class CheckCalculationResult
{
    public int PayrollRunEmployeeId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal GrossPay { get; set; }
    public decimal PreTaxDeductions { get; set; }
    public decimal TaxableWages { get; set; }
    public decimal EmployeeTaxes { get; set; }
    public decimal EmployerTaxes { get; set; }
    public decimal PostTaxDeductions { get; set; }
    public decimal Reimbursements { get; set; }
    public decimal NetPay { get; set; }
    public bool UsedConfiguredSandboxTaxService { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public sealed class CheckCalculationPreviewModel
{
    public PayrollRun? PayrollRun { get; set; }
    public List<CheckCalculationInput> Inputs { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class CheckCalculationPreviewService
{
    private readonly PayrollDbContext _context;
    private readonly ITaxCalculationService _configuredTaxService;
    private readonly FakeTaxCalculationService _fakeTaxService = new();
    private readonly LimitedLiabilityModeOptions _limitedLiabilityOptions;
    private readonly TaxApiOptions _taxApiOptions;

    public CheckCalculationPreviewService(
        PayrollDbContext context,
        ITaxCalculationService configuredTaxService,
        IOptions<LimitedLiabilityModeOptions> limitedLiabilityOptions,
        IOptions<TaxApiOptions> taxApiOptions)
    {
        _context = context;
        _configuredTaxService = configuredTaxService;
        _limitedLiabilityOptions = limitedLiabilityOptions.Value;
        _taxApiOptions = taxApiOptions.Value;
    }

    public async Task<CheckCalculationPreviewModel> LoadPreviewAsync(int payrollRunId)
    {
        var payrollRun = await LoadPayrollRunAsync(payrollRunId);
        var model = new CheckCalculationPreviewModel { PayrollRun = payrollRun };

        if (payrollRun == null)
        {
            model.Errors.Add("Payroll run was not found.");
            return model;
        }

        model.Errors.AddRange(GetBlockingErrors(payrollRun));
        model.Warnings.AddRange(GetWarnings(payrollRun));

        foreach (var check in payrollRun.PayrollRunEmployees.OrderBy(pre => pre.Employee?.LastName).ThenBy(pre => pre.Employee?.FirstName))
        {
            model.Inputs.Add(CreateInputFromExistingLines(check));
        }

        return model;
    }

    public async Task<CheckCalculationResult> RecalculateCheckAsync(
        int payrollRunId,
        CheckCalculationInput input,
        string performedByUserId = "local-prototype-user")
    {
        var payrollRun = await LoadPayrollRunAsync(payrollRunId)
            ?? throw new InvalidOperationException("Payroll run was not found.");

        EnsureCanRecalculate(payrollRun);
        ValidateInput(input);

        var check = payrollRun.PayrollRunEmployees.FirstOrDefault(pre => pre.PayrollRunEmployeeId == input.PayrollRunEmployeeId)
            ?? throw new InvalidOperationException("Employee check was not found for this payroll run.");

        var result = await CalculateCheckAsync(payrollRun, check, input);
        if (result.Errors.Count > 0)
        {
            return result;
        }

        SaveLines(check, payrollRun, input, result);
        await UpdateTotalsAsync(payrollRun);
        await AddAuditLogAsync(payrollRunId, "CheckRecalculated", $"Recalculated check for employee {check.EmployeeId}.", performedByUserId);

        return result;
    }

    public async Task<List<CheckCalculationResult>> RecalculateAllAsync(
        int payrollRunId,
        IEnumerable<CheckCalculationInput> inputs,
        string performedByUserId = "local-prototype-user")
    {
        var payrollRun = await LoadPayrollRunAsync(payrollRunId)
            ?? throw new InvalidOperationException("Payroll run was not found.");

        EnsureCanRecalculate(payrollRun);

        var results = new List<CheckCalculationResult>();
        foreach (var input in inputs)
        {
            ValidateInput(input);
            var check = payrollRun.PayrollRunEmployees.FirstOrDefault(pre => pre.PayrollRunEmployeeId == input.PayrollRunEmployeeId)
                ?? throw new InvalidOperationException("Employee check was not found for this payroll run.");

            var result = await CalculateCheckAsync(payrollRun, check, input);
            results.Add(result);
            if (result.Errors.Count == 0)
            {
                SaveLines(check, payrollRun, input, result);
            }
        }

        await UpdateTotalsAsync(payrollRun);
        await AddAuditLogAsync(payrollRunId, "PayrollChecksRecalculated", $"Recalculated {results.Count} check(s).", performedByUserId);

        return results;
    }

    private async Task<PayrollRun?> LoadPayrollRunAsync(int payrollRunId)
    {
        return await _context.PayrollRuns
            .Include(p => p.Company)
            .Include(p => p.PaySchedule)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.Employee)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.EarningLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.DeductionLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.TaxLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.NetPayLines)
            .Include(p => p.EmployerTaxLines)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);
    }

    private async Task<CheckCalculationResult> CalculateCheckAsync(PayrollRun payrollRun, PayrollRunEmployee check, CheckCalculationInput input)
    {
        var employee = check.Employee;
        var result = new CheckCalculationResult
        {
            PayrollRunEmployeeId = check.PayrollRunEmployeeId,
            EmployeeId = check.EmployeeId,
            EmployeeName = employee == null ? $"Employee {check.EmployeeId}" : $"{employee.FirstName} {employee.LastName}".Trim(),
            Reimbursements = Math.Round(input.Reimbursements, 2)
        };

        if (employee == null)
        {
            result.Errors.Add("Employee record was not found for this check.");
            return result;
        }

        var rate = employee.HourlyRate.GetValueOrDefault();
        var regularPay = employee.PayType == PayType.Hourly
            ? Math.Round(input.RegularHours * rate, 2)
            : CalculateSalaryPay(employee, payrollRun.PaySchedule);
        var overtimePay = employee.PayType == PayType.Hourly
            ? Math.Round(input.OvertimeHours * rate * 1.5m, 2)
            : 0m;

        result.GrossPay = Math.Round(regularPay + overtimePay + input.AdditionalEarnings, 2);
        result.PreTaxDeductions = 0m;
        result.TaxableWages = Math.Max(0m, Math.Round(result.GrossPay - result.PreTaxDeductions, 2));

        var taxService = ShouldUseConfiguredSandboxTaxService() ? _configuredTaxService : _fakeTaxService;
        result.UsedConfiguredSandboxTaxService = ReferenceEquals(taxService, _configuredTaxService);
        if (!result.UsedConfiguredSandboxTaxService)
        {
            result.Warnings.Add("Fake tax calculation service used for local preview.");
        }

        var taxes = await taxService.CalculateTaxesAsync(result.TaxableWages, employee.State, employee.State);
        result.EmployeeTaxes = taxes.TotalEmployeeTaxes;
        result.EmployerTaxes = taxes.TotalEmployerTaxes;
        result.PostTaxDeductions = Math.Round(input.ManualDeductions, 2);
        result.NetPay = Math.Round(result.GrossPay - result.PreTaxDeductions - result.EmployeeTaxes - result.PostTaxDeductions + result.Reimbursements, 2);

        if (result.NetPay < 0)
        {
            result.Errors.Add("Net pay is negative. Reduce deductions or adjust earnings before approval.");
        }

        if (result.EmployeeTaxes == 0)
        {
            result.Warnings.Add("Employee tax total is zero for this preview calculation.");
        }

        return result;
    }

    private void SaveLines(PayrollRunEmployee check, PayrollRun payrollRun, CheckCalculationInput input, CheckCalculationResult result)
    {
        _context.EarningLines.RemoveRange(check.EarningLines);
        _context.DeductionLines.RemoveRange(check.DeductionLines);
        _context.TaxLines.RemoveRange(check.TaxLines);
        _context.NetPayLines.RemoveRange(check.NetPayLines);
        var existingEmployerLines = payrollRun.EmployerTaxLines.Where(line => line.EmployeeId == check.EmployeeId).ToList();
        _context.EmployerTaxLines.RemoveRange(existingEmployerLines);
        foreach (var line in existingEmployerLines)
        {
            payrollRun.EmployerTaxLines.Remove(line);
        }

        var employee = check.Employee;
        var rate = employee?.HourlyRate.GetValueOrDefault() ?? 0m;

        if (input.RegularHours > 0 || employee?.PayType == PayType.Salary)
        {
            check.EarningLines.Add(new EarningLine
            {
                Description = employee?.PayType == PayType.Salary ? "Salary Pay" : "Regular Pay",
                Hours = employee?.PayType == PayType.Hourly ? input.RegularHours : null,
                Rate = employee?.PayType == PayType.Hourly ? rate : null,
                Amount = employee?.PayType == PayType.Salary ? CalculateSalaryPay(employee, payrollRun.PaySchedule) : Math.Round(input.RegularHours * rate, 2)
            });
        }

        if (input.OvertimeHours > 0)
        {
            check.EarningLines.Add(new EarningLine
            {
                Description = "Overtime Pay Placeholder",
                Hours = input.OvertimeHours,
                Rate = rate,
                Amount = Math.Round(input.OvertimeHours * rate * 1.5m, 2)
            });
        }

        if (input.AdditionalEarnings > 0)
        {
            check.EarningLines.Add(new EarningLine { Description = "Additional Earnings", Amount = Math.Round(input.AdditionalEarnings, 2) });
        }

        if (input.Reimbursements > 0)
        {
            check.EarningLines.Add(new EarningLine { Description = "Reimbursements", Amount = Math.Round(input.Reimbursements, 2) });
        }

        if (!string.IsNullOrWhiteSpace(input.Notes))
        {
            check.EarningLines.Add(new EarningLine { Description = $"Notes: {input.Notes.Trim()}", Amount = 0m });
        }

        if (result.PreTaxDeductions > 0)
        {
            check.DeductionLines.Add(new DeductionLine { Description = "Pre-Tax Deductions", Amount = result.PreTaxDeductions });
        }

        if (input.ManualDeductions > 0)
        {
            check.DeductionLines.Add(new DeductionLine { Description = "Manual Deductions", Amount = Math.Round(input.ManualDeductions, 2) });
        }

        check.TaxLines.Add(new TaxLine { TaxType = "Employee Taxes", Amount = result.EmployeeTaxes });
        check.NetPayLines.Add(new NetPayLine { Amount = result.NetPay, PaymentMethod = "Preview" });

        if (result.EmployerTaxes > 0)
        {
            payrollRun.EmployerTaxLines.Add(new EmployerTaxLine
            {
                PayrollRunId = payrollRun.PayrollRunId,
                EmployeeId = check.EmployeeId,
                TaxType = "Employer Taxes",
                Amount = result.EmployerTaxes
            });
        }

        check.GrossPay = result.GrossPay;
        check.TotalDeductions = result.PreTaxDeductions + result.PostTaxDeductions;
        check.TotalTaxes = result.EmployeeTaxes;
        check.NetPay = result.NetPay;
    }

    private async Task UpdateTotalsAsync(PayrollRun payrollRun)
    {
        payrollRun.TotalGrossPay = payrollRun.PayrollRunEmployees.Sum(check => check.GrossPay);
        payrollRun.TotalEmployeeTaxes = payrollRun.PayrollRunEmployees.Sum(check => check.TotalTaxes);
        payrollRun.TotalDeductions = payrollRun.PayrollRunEmployees.Sum(check => check.TotalDeductions);
        payrollRun.TotalNetPay = payrollRun.PayrollRunEmployees.Sum(check => check.NetPay);
        payrollRun.TotalEmployerTaxes = payrollRun.EmployerTaxLines.Sum(line => line.Amount);

        if (payrollRun.Status == PayrollStatus.Draft)
        {
            payrollRun.Status = PayrollStatus.Calculated;
        }

        await _context.SaveChangesAsync();
    }

    private static CheckCalculationInput CreateInputFromExistingLines(PayrollRunEmployee check)
    {
        return new CheckCalculationInput
        {
            PayrollRunEmployeeId = check.PayrollRunEmployeeId,
            RegularHours = check.EarningLines.Where(line => line.Description is "Regular Pay" or "Regular Hours").Sum(line => line.Hours ?? 0m),
            OvertimeHours = check.EarningLines.Where(line => line.Description.Contains("Overtime", StringComparison.OrdinalIgnoreCase)).Sum(line => line.Hours ?? 0m),
            AdditionalEarnings = check.EarningLines.Where(line => line.Description is "Additional Earnings" or "Manual gross pay adjustment" or "Manual Adjustment").Sum(line => line.Amount),
            Reimbursements = check.EarningLines.Where(line => line.Description == "Reimbursements").Sum(line => line.Amount),
            ManualDeductions = check.DeductionLines.Where(line => line.Description == "Manual Deductions").Sum(line => line.Amount),
            Notes = check.EarningLines.FirstOrDefault(line => line.Description.StartsWith("Notes:", StringComparison.OrdinalIgnoreCase))?.Description.Replace("Notes:", string.Empty).Trim() ?? string.Empty
        };
    }

    private static decimal CalculateSalaryPay(Employee? employee, PaySchedule? paySchedule)
    {
        if (employee == null || paySchedule == null)
        {
            return 0m;
        }

        var periods = paySchedule.Frequency switch
        {
            PayFrequency.Weekly => 52m,
            PayFrequency.Biweekly => 26m,
            PayFrequency.Semimonthly => 24m,
            PayFrequency.Monthly => 12m,
            _ => 26m
        };

        return Math.Round(employee.AnnualSalary.GetValueOrDefault() / periods, 2);
    }

    private bool ShouldUseConfiguredSandboxTaxService()
    {
        return _limitedLiabilityOptions.AllowExternalTaxApiLookup && _taxApiOptions.SandboxMode;
    }

    private static void EnsureCanRecalculate(PayrollRun payrollRun)
    {
        var errors = GetBlockingErrors(payrollRun);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(errors[0]);
        }
    }

    private static List<string> GetBlockingErrors(PayrollRun payrollRun)
    {
        var errors = new List<string>();
        if (payrollRun.Status is PayrollStatus.Approved or PayrollStatus.Submitted or PayrollStatus.Completed)
        {
            errors.Add("Approved, submitted, or completed payroll runs cannot be recalculated from this page.");
        }

        if (!payrollRun.PayrollRunEmployees.Any())
        {
            errors.Add("Payroll run has no employee checks.");
        }

        return errors;
    }

    private static List<string> GetWarnings(PayrollRun payrollRun)
    {
        var warnings = new List<string>();
        if (payrollRun.PayDate.Date < DateTime.Today)
        {
            warnings.Add("Pay date is in the past.");
        }

        return warnings;
    }

    private static void ValidateInput(CheckCalculationInput input)
    {
        if (input.RegularHours < 0 || input.OvertimeHours < 0 || input.AdditionalEarnings < 0 || input.ManualDeductions < 0 || input.Reimbursements < 0)
        {
            throw new InvalidOperationException("Check input amounts cannot be negative.");
        }
    }

    private async Task AddAuditLogAsync(int payrollRunId, string eventType, string description, string createdByUserId)
    {
        _context.AuditLogEntries.Add(new AuditLogEntry
        {
            PayrollRunId = payrollRunId,
            EventType = eventType,
            Description = description,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}
