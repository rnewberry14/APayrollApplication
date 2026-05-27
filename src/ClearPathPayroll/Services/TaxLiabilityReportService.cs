using System.Text;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Builds tax liability reports for payroll runs and company pay date filters.
/// </summary>
public class TaxLiabilityReportService
{
    private readonly PayrollDbContext _context;

    public TaxLiabilityReportService(PayrollDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets active companies for filtering tax liability reports.
    /// </summary>
    public async Task<List<Company>> GetAvailableCompaniesAsync()
    {
        return await _context.Companies
            .Where(c => c.IsActive)
            .OrderBy(c => c.LegalName)
            .ToListAsync();
    }

    /// <summary>
    /// Builds a tax liability report for the selected company and pay date range.
    /// </summary>
    public async Task<TaxLiabilityReportModel> GetTaxLiabilityReportAsync(int? companyId, DateTime? payDateStart, DateTime? payDateEnd)
    {
        var query = _context.PayrollRuns
            .AsNoTracking()
            .Include(p => p.Company)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.TaxLines)
            .Include(p => p.EmployerTaxLines)
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

        var report = new TaxLiabilityReportModel
        {
            CompanyId = companyId,
            CompanyName = companyName,
            PayDateStart = payDateStart,
            PayDateEnd = payDateEnd,
            Runs = payrollRuns.Select(run => new TaxLiabilityRun
            {
                PayrollRunId = run.PayrollRunId,
                CompanyName = run.Company?.LegalName ?? string.Empty,
                PayDate = run.PayDate,
                PayPeriodStart = run.PayPeriodStart,
                PayPeriodEnd = run.PayPeriodEnd,
                Status = run.Status,
                DueDatePlaceholder = "TBD",
                PaymentStatusPlaceholder = "Pending review",
                EmployeeWithholdingGroups = run.PayrollRunEmployees
                    .SelectMany(emp => emp.TaxLines)
                    .Where(tax => !string.IsNullOrWhiteSpace(tax.TaxType))
                    .GroupBy(tax => tax.TaxType)
                    .Select(group => new TaxLiabilityGroup
                    {
                        TaxType = group.Key,
                        Amount = group.Sum(tax => tax.Amount)
                    })
                    .OrderBy(group => group.TaxType)
                    .ToList(),
                EmployerTaxGroups = run.EmployerTaxLines
                    .Where(tax => !string.IsNullOrWhiteSpace(tax.TaxType))
                    .GroupBy(tax => tax.TaxType)
                    .Select(group => new TaxLiabilityGroup
                    {
                        TaxType = group.Key,
                        Amount = group.Sum(tax => tax.Amount)
                    })
                    .OrderBy(group => group.TaxType)
                    .ToList()
            }).ToList()
        };

        var depositQuery = _context.TaxDepositRecords
            .AsNoTracking()
            .Include(deposit => deposit.Company)
            .AsQueryable();

        if (companyId.HasValue)
        {
            depositQuery = depositQuery.Where(deposit => deposit.CompanyId == companyId.Value);
        }

        if (payDateStart.HasValue)
        {
            depositQuery = depositQuery.Where(deposit => deposit.DepositDate >= payDateStart.Value.Date);
        }

        if (payDateEnd.HasValue)
        {
            depositQuery = depositQuery.Where(deposit => deposit.DepositDate <= payDateEnd.Value.Date);
        }

        report.DepositRecords = await depositQuery
            .OrderByDescending(deposit => deposit.DepositDate)
            .ThenBy(deposit => deposit.TaxType)
            .Select(deposit => new TaxDepositReportRecord
            {
                TaxDepositRecordId = deposit.TaxDepositRecordId,
                CompanyName = deposit.Company == null ? string.Empty : deposit.Company.LegalName,
                DepositDate = deposit.DepositDate,
                TaxPeriodStart = deposit.TaxPeriodStart,
                TaxPeriodEnd = deposit.TaxPeriodEnd,
                TaxType = deposit.TaxType,
                Agency = deposit.Agency,
                Amount = deposit.Amount,
                ConfirmationNumber = deposit.ConfirmationNumber,
                PaymentMethod = deposit.PaymentMethod,
                Notes = deposit.Notes,
                RecordSource = deposit.RecordSource
            })
            .ToListAsync();

        foreach (var run in report.Runs)
        {
            run.TotalEmployeeWithholding = run.EmployeeWithholdingGroups.Sum(g => g.Amount);
            run.TotalEmployerTaxes = run.EmployerTaxGroups.Sum(g => g.Amount);
            run.TotalTaxLiability = run.TotalEmployeeWithholding + run.TotalEmployerTaxes;
        }

        report.TotalEmployeeWithholding = report.Runs.Sum(r => r.TotalEmployeeWithholding);
        report.TotalEmployerTaxes = report.Runs.Sum(r => r.TotalEmployerTaxes);
        report.TotalTaxLiability = report.Runs.Sum(r => r.TotalTaxLiability);
        report.TotalUserEnteredDeposits = report.DepositRecords.Sum(record => record.Amount);
        report.GeneratedAt = DateTime.UtcNow;

        return report;
    }

    /// <summary>
    /// Converts a tax liability report to CSV text.
    /// </summary>
    public string BuildTaxLiabilityCsv(TaxLiabilityReportModel report)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Tax Liability Report");
        csv.AppendLine($"Company,{EscapeCsvValue(report.CompanyName)}");
        csv.AppendLine($"Pay Date Start,{(report.PayDateStart?.ToString("yyyy-MM-dd") ?? string.Empty)}");
        csv.AppendLine($"Pay Date End,{(report.PayDateEnd?.ToString("yyyy-MM-dd") ?? string.Empty)}");
        csv.AppendLine($"Generated At,{report.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
        csv.AppendLine();
        csv.AppendLine("Payroll Run ID,Company,Pay Date,Due Date,Payment Status,Liability Type,Tax Type,Amount");

        foreach (var run in report.Runs)
        {
            foreach (var group in run.EmployeeWithholdingGroups)
            {
                csv.AppendLine(string.Join(",",
                    EscapeCsvValue(run.PayrollRunId.ToString()),
                    EscapeCsvValue(run.CompanyName),
                    EscapeCsvValue(run.PayDate.ToString("yyyy-MM-dd")),
                    EscapeCsvValue(run.DueDatePlaceholder),
                    EscapeCsvValue(run.PaymentStatusPlaceholder),
                    EscapeCsvValue("Employee Withholding"),
                    EscapeCsvValue(group.TaxType),
                    group.Amount.ToString("F2")
                ));
            }

            foreach (var group in run.EmployerTaxGroups)
            {
                csv.AppendLine(string.Join(",",
                    EscapeCsvValue(run.PayrollRunId.ToString()),
                    EscapeCsvValue(run.CompanyName),
                    EscapeCsvValue(run.PayDate.ToString("yyyy-MM-dd")),
                    EscapeCsvValue(run.DueDatePlaceholder),
                    EscapeCsvValue(run.PaymentStatusPlaceholder),
                    EscapeCsvValue("Employer Tax"),
                    EscapeCsvValue(group.TaxType),
                    group.Amount.ToString("F2")
                ));
            }

            csv.AppendLine(string.Join(",",
                EscapeCsvValue(run.PayrollRunId.ToString()),
                EscapeCsvValue(run.CompanyName),
                EscapeCsvValue(run.PayDate.ToString("yyyy-MM-dd")),
                EscapeCsvValue(run.DueDatePlaceholder),
                EscapeCsvValue(run.PaymentStatusPlaceholder),
                EscapeCsvValue("Run Total"),
                string.Empty,
                run.TotalTaxLiability.ToString("F2")
            ));
        }

        csv.AppendLine();
        csv.AppendLine("User-entered deposit records");
        csv.AppendLine("Company,Deposit Date,Tax Period Start,Tax Period End,Tax Type,Agency,Amount,Confirmation Number,Payment Method,Record Source,Notes");
        foreach (var deposit in report.DepositRecords)
        {
            csv.AppendLine(string.Join(",",
                EscapeCsvValue(deposit.CompanyName),
                EscapeCsvValue(deposit.DepositDate.ToString("yyyy-MM-dd")),
                EscapeCsvValue(deposit.TaxPeriodStart.ToString("yyyy-MM-dd")),
                EscapeCsvValue(deposit.TaxPeriodEnd.ToString("yyyy-MM-dd")),
                EscapeCsvValue(deposit.TaxType),
                EscapeCsvValue(deposit.Agency),
                deposit.Amount.ToString("F2"),
                EscapeCsvValue(deposit.ConfirmationNumber ?? string.Empty),
                EscapeCsvValue(deposit.PaymentMethod),
                EscapeCsvValue(deposit.RecordSource),
                EscapeCsvValue(deposit.Notes ?? string.Empty)
            ));
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
            report.TotalTaxLiability.ToString("F2")
        ));
        csv.AppendLine($"User-entered deposits total,,,,,,,{report.TotalUserEnteredDeposits:F2}");

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
/// Model for the tax liability report output.
/// </summary>
public class TaxLiabilityReportModel
{
    public int? CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime? PayDateStart { get; set; }
    public DateTime? PayDateEnd { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<TaxLiabilityRun> Runs { get; set; } = new();
    public List<TaxDepositReportRecord> DepositRecords { get; set; } = new();
    public decimal TotalEmployeeWithholding { get; set; }
    public decimal TotalEmployerTaxes { get; set; }
    public decimal TotalTaxLiability { get; set; }
    public decimal TotalUserEnteredDeposits { get; set; }
}

/// <summary>
/// A run-level entry in the tax liability report.
/// </summary>
public class TaxLiabilityRun
{
    public int PayrollRunId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime PayDate { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public PayrollStatus Status { get; set; }
    public string DueDatePlaceholder { get; set; } = string.Empty;
    public string PaymentStatusPlaceholder { get; set; } = string.Empty;
    public List<TaxLiabilityGroup> EmployeeWithholdingGroups { get; set; } = new();
    public List<TaxLiabilityGroup> EmployerTaxGroups { get; set; } = new();
    public decimal TotalEmployeeWithholding { get; set; }
    public decimal TotalEmployerTaxes { get; set; }
    public decimal TotalTaxLiability { get; set; }
}

/// <summary>
/// A grouped tax liability amount by tax type.
/// </summary>
public class TaxLiabilityGroup
{
    public string TaxType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class TaxDepositReportRecord
{
    public int TaxDepositRecordId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime DepositDate { get; set; }
    public DateTime TaxPeriodStart { get; set; }
    public DateTime TaxPeriodEnd { get; set; }
    public string TaxType { get; set; } = string.Empty;
    public string Agency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ConfirmationNumber { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string RecordSource { get; set; } = "User-entered deposit record";
}
