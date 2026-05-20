using System.Text;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Builds payroll register reports for company and pay date range filters.
/// </summary>
public class PayrollRegisterService
{
    private readonly PayrollDbContext _context;

    public PayrollRegisterService(PayrollDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets active companies for filtering payroll register reports.
    /// </summary>
    public async Task<List<Company>> GetAvailableCompaniesAsync()
    {
        return await _context.Companies
            .Where(c => c.IsActive)
            .OrderBy(c => c.LegalName)
            .ToListAsync();
    }

    /// <summary>
    /// Builds a payroll register report for the selected filters.
    /// </summary>
    public async Task<PayrollRegisterModel> GetPayrollRegisterAsync(int? companyId, DateTime? payDateStart, DateTime? payDateEnd)
    {
        var query = _context.PayrollRuns
            .AsNoTracking()
            .Include(p => p.Company)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.Employee)
            .AsQueryable();

        if (companyId.HasValue)
        {
            query = query.Where(p => p.CompanyId == companyId.Value);
        }

        if (payDateStart.HasValue)
        {
            query = query.Where(p => p.PayDate >= payDateStart.Value.Date);
        }

        if (payDateEnd.HasValue)
        {
            query = query.Where(p => p.PayDate <= payDateEnd.Value.Date);
        }

        var payrollRuns = await query
            .OrderByDescending(p => p.PayDate)
            .ThenByDescending(p => p.PayrollRunId)
            .ToListAsync();

        var companyName = "All Companies";
        if (companyId.HasValue)
        {
            var company = await _context.Companies.FindAsync(companyId.Value);
            companyName = company?.LegalName ?? "Unknown Company";
        }

        var register = new PayrollRegisterModel
        {
            CompanyId = companyId,
            CompanyName = companyName,
            PayDateStart = payDateStart,
            PayDateEnd = payDateEnd,
            Runs = payrollRuns.Select(run => new PayrollRegisterRun
            {
                PayrollRunId = run.PayrollRunId,
                CompanyName = run.Company?.LegalName ?? string.Empty,
                PayDate = run.PayDate,
                PayPeriodStart = run.PayPeriodStart,
                PayPeriodEnd = run.PayPeriodEnd,
                Status = run.Status,
                TotalGrossPay = run.TotalGrossPay,
                TotalEmployeeTaxes = run.TotalEmployeeTaxes,
                TotalEmployerTaxes = run.TotalEmployerTaxes,
                TotalDeductions = run.TotalDeductions,
                TotalNetPay = run.TotalNetPay,
                EmployeeSummaries = run.PayrollRunEmployees
                    .OrderBy(emp => emp.Employee?.LastName)
                    .ThenBy(emp => emp.Employee?.FirstName)
                    .Select(emp => new PayrollRegisterEmployeeSummary
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeName = emp.Employee is not null
                            ? $"{emp.Employee.FirstName} {emp.Employee.LastName}".Trim()
                            : "Unknown Employee",
                        GrossPay = emp.GrossPay,
                        TotalTaxes = emp.TotalTaxes,
                        TotalDeductions = emp.TotalDeductions,
                        NetPay = emp.NetPay
                    })
                    .ToList()
            }).ToList()
        };

        register.TotalGrossWages = register.Runs.Sum(r => r.TotalGrossPay);
        register.TotalEmployeeTaxes = register.Runs.Sum(r => r.TotalEmployeeTaxes);
        register.TotalEmployerTaxes = register.Runs.Sum(r => r.TotalEmployerTaxes);
        register.TotalDeductions = register.Runs.Sum(r => r.TotalDeductions);
        register.TotalNetPay = register.Runs.Sum(r => r.TotalNetPay);
        register.GeneratedAt = DateTime.UtcNow;

        return register;
    }

    /// <summary>
    /// Converts a payroll register report into CSV text.
    /// </summary>
    public string BuildPayrollRegisterCsv(PayrollRegisterModel report)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Payroll Register Report");
        csv.AppendLine($"Company,{EscapeCsvValue(report.CompanyName)}");
        csv.AppendLine($"Pay Date Start,{(report.PayDateStart?.ToString("yyyy-MM-dd") ?? string.Empty)}");
        csv.AppendLine($"Pay Date End,{(report.PayDateEnd?.ToString("yyyy-MM-dd") ?? string.Empty)}");
        csv.AppendLine($"Generated At,{report.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
        csv.AppendLine();
        csv.AppendLine("Payroll Run ID,Company,Pay Date,Pay Period Start,Pay Period End,Status,Employee Name,Gross Pay,Employee Taxes,Employer Taxes,Deductions,Net Pay");

        foreach (var run in report.Runs)
        {
            if (run.EmployeeSummaries.Any())
            {
                foreach (var employee in run.EmployeeSummaries)
                {
                    csv.AppendLine(string.Join(",",
                        EscapeCsvValue(run.PayrollRunId.ToString()),
                        EscapeCsvValue(run.CompanyName),
                        EscapeCsvValue(run.PayDate.ToString("yyyy-MM-dd")),
                        EscapeCsvValue(run.PayPeriodStart.ToString("yyyy-MM-dd")),
                        EscapeCsvValue(run.PayPeriodEnd.ToString("yyyy-MM-dd")),
                        EscapeCsvValue(run.Status.ToString()),
                        EscapeCsvValue(employee.EmployeeName),
                        employee.GrossPay.ToString("F2"),
                        employee.TotalTaxes.ToString("F2"),
                        run.TotalEmployerTaxes.ToString("F2"),
                        employee.TotalDeductions.ToString("F2"),
                        employee.NetPay.ToString("F2")
                    ));
                }
            }
            else
            {
                csv.AppendLine(string.Join(",",
                    EscapeCsvValue(run.PayrollRunId.ToString()),
                    EscapeCsvValue(run.CompanyName),
                    EscapeCsvValue(run.PayDate.ToString("yyyy-MM-dd")),
                    EscapeCsvValue(run.PayPeriodStart.ToString("yyyy-MM-dd")),
                    EscapeCsvValue(run.PayPeriodEnd.ToString("yyyy-MM-dd")),
                    EscapeCsvValue(run.Status.ToString()),
                    string.Empty,
                    run.TotalGrossPay.ToString("F2"),
                    run.TotalEmployeeTaxes.ToString("F2"),
                    run.TotalEmployerTaxes.ToString("F2"),
                    run.TotalDeductions.ToString("F2"),
                    run.TotalNetPay.ToString("F2")
                ));
            }
        }

        csv.AppendLine();
        csv.AppendLine(string.Join(",",
            "Totals",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            report.TotalGrossWages.ToString("F2"),
            report.TotalEmployeeTaxes.ToString("F2"),
            report.TotalEmployerTaxes.ToString("F2"),
            report.TotalDeductions.ToString("F2"),
            report.TotalNetPay.ToString("F2")
        ));

        return csv.ToString();
    }

    private static string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");
        return escaped.Contains(',') || escaped.Contains('"') || escaped.Contains('\n')
            ? $"\"{escaped}\""
            : escaped;
    }
}

/// <summary>
/// Model for payroll register report output.
/// </summary>
public class PayrollRegisterModel
{
    public int? CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime? PayDateStart { get; set; }
    public DateTime? PayDateEnd { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<PayrollRegisterRun> Runs { get; set; } = new();
    public decimal TotalGrossWages { get; set; }
    public decimal TotalEmployeeTaxes { get; set; }
    public decimal TotalEmployerTaxes { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetPay { get; set; }
}

/// <summary>
/// Summary for a single payroll run in the register.
/// </summary>
public class PayrollRegisterRun
{
    public int PayrollRunId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime PayDate { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public PayrollStatus Status { get; set; }
    public decimal TotalGrossPay { get; set; }
    public decimal TotalEmployeeTaxes { get; set; }
    public decimal TotalEmployerTaxes { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetPay { get; set; }
    public List<PayrollRegisterEmployeeSummary> EmployeeSummaries { get; set; } = new();
}

/// <summary>
/// Employee-level summary row in the payroll register.
/// </summary>
public class PayrollRegisterEmployeeSummary
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal GrossPay { get; set; }
    public decimal TotalTaxes { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
}
