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

    /// <summary>
    /// Gets pay stub details for a specific employee in a payroll run.
    /// </summary>
    public async Task<PayStubModel> GetPayStubAsync(int payrollRunId, int employeeId)
    {
        var payrollRunEmployee = await _context.PayrollRunEmployees
            .Include(pre => pre.Employee)
            .Include(pre => pre.PayrollRun)
                .ThenInclude(pr => pr.Company)
            .Include(pre => pre.EarningLines)
            .Include(pre => pre.DeductionLines)
            .Include(pre => pre.TaxLines)
            .Include(pre => pre.NetPayLines)
            .FirstOrDefaultAsync(pre => pre.PayrollRunId == payrollRunId && pre.EmployeeId == employeeId);

        if (payrollRunEmployee == null)
        {
            throw new InvalidOperationException($"Pay stub not found for payroll run {payrollRunId} and employee {employeeId}.");
        }

        var employee = payrollRunEmployee.Employee ?? throw new InvalidOperationException($"Employee {employeeId} data missing for pay stub.");
        var payrollRun = payrollRunEmployee.PayrollRun ?? throw new InvalidOperationException($"Payroll run {payrollRunId} data missing for pay stub.");
        var company = payrollRun.Company ?? throw new InvalidOperationException($"Company data missing for pay stub.");

        var payDate = payrollRun.PayDate;
        var payPeriodStart = payrollRun.PayPeriodStart;
        var payPeriodEnd = payrollRun.PayPeriodEnd;

        var directDepositItem = payrollRunEmployee.NetPayLines
            .FirstOrDefault(n => n.PaymentMethod == "DirectDeposit");

        var directDepositLast4 = "N/A";
        if (directDepositItem != null)
        {
            var bank = await _context.EmployeeBankAccounts
                .Where(b => b.EmployeeId == employeeId && b.IsActive && b.VerificationStatus == VerificationStatus.Verified)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefaultAsync();

            if (bank != null)
            {
                directDepositLast4 = bank.Last4;
            }
            else
            {
                directDepositLast4 = "Unavailable";
            }
        }

        return new PayStubModel
        {
            PayrollRunId = payrollRunId,
            EmployeeId = employeeId,
            CompanyName = company.LegalName,
            EmployeeName = $"{employee.FirstName} {employee.LastName}".Trim(),
            SsnLast4 = employee.SSNLast4,
            PayPeriodStart = payPeriodStart,
            PayPeriodEnd = payPeriodEnd,
            PayDate = payDate,
            Earnings = payrollRunEmployee.EarningLines
                .Select(e => new PayStubLineItem(e.Description, e.Amount))
                .ToList(),
            EmployeeTaxes = payrollRunEmployee.TaxLines
                .Select(t => new PayStubLineItem(t.TaxType, t.Amount))
                .ToList(),
            Deductions = payrollRunEmployee.DeductionLines
                .Select(d => new PayStubLineItem(d.Description, d.Amount))
                .ToList(),
            NetPay = payrollRunEmployee.NetPay,
            DirectDepositLast4 = directDepositLast4,
            YtdTotalsPlaceholder = "Year-to-date totals will be added in a future release.",
            YtdGrossPay = 0m,
            YtdTaxes = 0m,
            YtdDeductions = 0m,
            YtdNetPay = 0m
        };
    }
}

/// <summary>
/// Represents a pay stub model for display and printing.
/// </summary>
public class PayStubModel
{
    public int PayrollRunId { get; set; }
    public int EmployeeId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string SsnLast4 { get; set; } = string.Empty;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public DateTime PayDate { get; set; }
    public List<PayStubLineItem> Earnings { get; set; } = new();
    public List<PayStubLineItem> EmployeeTaxes { get; set; } = new();
    public List<PayStubLineItem> Deductions { get; set; } = new();
    public decimal NetPay { get; set; }
    public string DirectDepositLast4 { get; set; } = string.Empty;
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
    public PayStubLineItem(string description, decimal amount)
    {
        Description = description;
        Amount = amount;
    }

    public string Description { get; set; }
    public decimal Amount { get; set; }
}
