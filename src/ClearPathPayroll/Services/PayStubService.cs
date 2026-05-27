using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Generates pay stub data for a payroll run employee.
/// </summary>
public class PayStubService
{
    private readonly PayrollDbContext _context;

    public PayStubService(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task<PayStubModel> GetPayStubByPayrollRunEmployeeIdAsync(int payrollRunEmployeeId)
    {
        var payrollRunEmployee = await _context.PayrollRunEmployees
            .Include(pre => pre.Employee)
            .Include(pre => pre.PayrollRun)
                .ThenInclude(pr => pr!.Company)
            .Include(pre => pre.EarningLines)
            .Include(pre => pre.DeductionLines)
            .Include(pre => pre.TaxLines)
            .Include(pre => pre.NetPayLines)
            .FirstOrDefaultAsync(pre => pre.PayrollRunEmployeeId == payrollRunEmployeeId);

        if (payrollRunEmployee == null)
        {
            throw new InvalidOperationException($"Pay stub not found for payroll run employee {payrollRunEmployeeId}.");
        }

        var employee = payrollRunEmployee.Employee ?? throw new InvalidOperationException($"Employee {payrollRunEmployee.EmployeeId} data missing for pay stub.");
        var payrollRun = payrollRunEmployee.PayrollRun ?? throw new InvalidOperationException($"Payroll run {payrollRunEmployee.PayrollRunId} data missing for pay stub.");
        var company = payrollRun.Company ?? throw new InvalidOperationException($"Company data missing for pay stub.");

        var directDepositItem = payrollRunEmployee.NetPayLines
            .FirstOrDefault(n => string.Equals(n.PaymentMethod, "DirectDeposit", StringComparison.OrdinalIgnoreCase));
        var checkItem = payrollRunEmployee.NetPayLines
            .FirstOrDefault(n => string.Equals(n.PaymentMethod, "Check", StringComparison.OrdinalIgnoreCase));

        var directDepositLast4 = "N/A";
        if (directDepositItem != null)
        {
            var bank = await _context.EmployeeBankAccounts
                .Where(b => b.EmployeeId == employee.EmployeeId && b.IsActive && b.VerificationStatus == VerificationStatus.Verified)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefaultAsync();

            directDepositLast4 = bank?.Last4 ?? "Unavailable";
        }

        var employerTaxes = await _context.EmployerTaxLines
            .Where(t => t.PayrollRunId == payrollRun.PayrollRunId && t.EmployeeId == employee.EmployeeId)
            .ToListAsync();

        var earnings = payrollRunEmployee.EarningLines
            .Where(e => !string.Equals(e.Description, "Reimbursements", StringComparison.OrdinalIgnoreCase)
                && !e.Description.StartsWith("Notes:", StringComparison.OrdinalIgnoreCase))
            .Select(e => new PayStubLineItem(e.Description, e.Amount, e.Hours, e.Rate))
            .ToList();

        var reimbursements = payrollRunEmployee.EarningLines
            .Where(e => string.Equals(e.Description, "Reimbursements", StringComparison.OrdinalIgnoreCase))
            .Select(e => new PayStubLineItem(e.Description, e.Amount, e.Hours, e.Rate))
            .ToList();

        var preTaxDeductions = payrollRunEmployee.DeductionLines
            .Where(d => d.Description.Contains("Pre-Tax", StringComparison.OrdinalIgnoreCase)
                || d.Description.Contains("Pretax", StringComparison.OrdinalIgnoreCase))
            .Select(d => new PayStubLineItem(d.Description, d.Amount))
            .ToList();

        var postTaxDeductions = payrollRunEmployee.DeductionLines
            .Where(d => !d.Description.Contains("Pre-Tax", StringComparison.OrdinalIgnoreCase)
                && !d.Description.Contains("Pretax", StringComparison.OrdinalIgnoreCase))
            .Select(d => new PayStubLineItem(d.Description, d.Amount))
            .ToList();

        return new PayStubModel
        {
            PayrollRunEmployeeId = payrollRunEmployee.PayrollRunEmployeeId,
            PayrollRunId = payrollRun.PayrollRunId,
            EmployeeId = employee.EmployeeId,
            CompanyName = company.LegalName,
            CompanyAddress = FormatAddress(company.PrimaryAddress, company.City, company.State, company.ZipCode),
            EmployeeName = $"{employee.FirstName} {employee.LastName}".Trim(),
            EmployeeAddress = FormatAddress(employee.Address1 ?? employee.ResidenceAddress, employee.City, employee.State, employee.ZipCode),
            SsnLast4 = employee.SSNLast4,
            PayPeriodStart = payrollRun.PayPeriodStart,
            PayPeriodEnd = payrollRun.PayPeriodEnd,
            PayDate = payrollRun.PayDate,
            Earnings = earnings,
            GrossPay = payrollRunEmployee.GrossPay,
            EmployeeTaxes = payrollRunEmployee.TaxLines
                .Select(t => new PayStubLineItem(t.TaxType, t.Amount))
                .ToList(),
            EmployerTaxes = employerTaxes
                .Select(t => new PayStubLineItem(t.TaxType, t.Amount))
                .ToList(),
            PreTaxDeductions = preTaxDeductions,
            PostTaxDeductions = postTaxDeductions,
            Deductions = preTaxDeductions.Concat(postTaxDeductions).ToList(),
            Reimbursements = reimbursements,
            NetPay = payrollRunEmployee.NetPay,
            DirectDepositLast4 = directDepositLast4,
            PaymentMethod = directDepositItem != null ? "DirectDeposit" : checkItem?.PaymentMethod ?? payrollRunEmployee.NetPayLines.FirstOrDefault()?.PaymentMethod ?? "Not set",
            CheckNumberPlaceholder = checkItem != null ? $"CHK-{payrollRun.PayrollRunId}-{employee.EmployeeId}" : string.Empty,
            EmployerNotes = company.EmployerTaxNotes ?? string.Empty,
            RequiredFieldsChecklistPlaceholder = "State-required-fields checklist placeholder. Review applicable pay statement requirements before distribution.",
            AccrualsPlaceholder = "Accrual placeholders are available for local tracking.",
            YtdTotalsPlaceholder = "Year-to-date placeholders are available for local tracking.",
            YtdGrossPay = 0m,
            YtdTaxes = 0m,
            YtdDeductions = 0m,
            YtdNetPay = 0m
        };
    }

    /// <summary>
    /// Gets pay stub details for a specific employee in a payroll run.
    /// </summary>
    public async Task<PayStubModel> GetPayStubAsync(int payrollRunId, int employeeId)
    {
        var payrollRunEmployeeId = await _context.PayrollRunEmployees
            .Where(pre => pre.PayrollRunId == payrollRunId && pre.EmployeeId == employeeId)
            .Select(pre => pre.PayrollRunEmployeeId)
            .FirstOrDefaultAsync();

        if (payrollRunEmployeeId == 0)
        {
            throw new InvalidOperationException($"Pay stub not found for payroll run {payrollRunId} and employee {employeeId}.");
        }

        return await GetPayStubByPayrollRunEmployeeIdAsync(payrollRunEmployeeId);
    }

    private static string FormatAddress(string? address, string? city, string? state, string? zipCode)
    {
        var cityState = string.Join(", ", new[] { city, state }.Where(part => !string.IsNullOrWhiteSpace(part)));
        var cityStateZip = string.Join(" ", new[] { cityState, zipCode }.Where(part => !string.IsNullOrWhiteSpace(part)));
        return string.Join(Environment.NewLine, new[] { address, cityStateZip }.Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}

/// <summary>
/// Represents a pay stub model for display and printing.
/// </summary>
public class PayStubModel
{
    public int PayrollRunEmployeeId { get; set; }
    public int PayrollRunId { get; set; }
    public int EmployeeId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeAddress { get; set; } = string.Empty;
    public string SsnLast4 { get; set; } = string.Empty;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public DateTime PayDate { get; set; }
    public List<PayStubLineItem> Earnings { get; set; } = new();
    public decimal GrossPay { get; set; }
    public List<PayStubLineItem> EmployeeTaxes { get; set; } = new();
    public List<PayStubLineItem> EmployerTaxes { get; set; } = new();
    public List<PayStubLineItem> PreTaxDeductions { get; set; } = new();
    public List<PayStubLineItem> PostTaxDeductions { get; set; } = new();
    public List<PayStubLineItem> Deductions { get; set; } = new();
    public List<PayStubLineItem> Reimbursements { get; set; } = new();
    public decimal NetPay { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string DirectDepositLast4 { get; set; } = string.Empty;
    public string CheckNumberPlaceholder { get; set; } = string.Empty;
    public string EmployerNotes { get; set; } = string.Empty;
    public string RequiredFieldsChecklistPlaceholder { get; set; } = string.Empty;
    public string AccrualsPlaceholder { get; set; } = string.Empty;
    public string YtdTotalsPlaceholder { get; set; } = string.Empty;
    public decimal YtdGrossPay { get; set; }
    public decimal YtdTaxes { get; set; }
    public decimal YtdDeductions { get; set; }
    public decimal YtdNetPay { get; set; }
}

/// <summary>
/// Represents a single line item on the pay stub.
/// </summary>
public class PayStubLineItem
{
    public PayStubLineItem(string description, decimal amount, decimal? hours = null, decimal? rate = null)
    {
        Description = description;
        Amount = amount;
        Hours = hours;
        Rate = rate;
    }

    public string Description { get; set; }
    public decimal Amount { get; set; }
    public decimal? Hours { get; set; }
    public decimal? Rate { get; set; }
}

public class PayStubPrintSettings
{
    public bool ShowHours { get; set; } = true;
    public bool ShowRates { get; set; } = true;
    public bool ShowYtd { get; set; } = true;
    public bool ShowEmployerTaxes { get; set; }
    public bool ShowEmployeeAddress { get; set; } = true;
    public bool ShowAccrualPlaceholders { get; set; } = true;
    public bool ShowStateRequiredFieldsChecklistPlaceholder { get; set; } = true;
}
