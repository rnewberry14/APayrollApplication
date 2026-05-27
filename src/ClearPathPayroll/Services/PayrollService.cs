using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Service for managing payroll runs.
/// </summary>
public class PayrollService
{
    private readonly PayrollDbContext _context;

    public PayrollService(PayrollDbContext context)
    {
        _context = context;
    }

    public sealed class PayrollRunEmployeeSetupInput
    {
        public int EmployeeId { get; set; }
        public decimal RegularHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal ManualGrossPayAdjustment { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string PayrollMode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Gets all payroll runs for a company.
    /// </summary>
    public async Task<List<PayrollRun>> GetPayrollRunsByCompanyAsync(int companyId)
    {
        return await _context.PayrollRuns
            .Where(p => p.CompanyId == companyId)
            .Include(p => p.PaySchedule)
            .Include(p => p.PayrollRunEmployees)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a payroll run by ID.
    /// </summary>
    public async Task<PayrollRun?> GetPayrollRunByIdAsync(int id)
    {
        return await _context.PayrollRuns
            .Include(p => p.PaySchedule)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.EarningLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.DeductionLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.TaxLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.NetPayLines)
            .Include(p => p.EmployerTaxLines)
            .FirstOrDefaultAsync(p => p.PayrollRunId == id);
    }

    /// <summary>
    /// Creates a new payroll run in Draft status.
    /// </summary>
    public async Task<PayrollRun> CreatePayrollRunAsync(PayrollRun payrollRun)
    {
        payrollRun.Status = PayrollStatus.Draft;
        payrollRun.CreatedAt = DateTime.UtcNow;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();
        return payrollRun;
    }

    public async Task<PayrollRun> CreateDraftPayrollRunAsync(
        int companyId,
        int payScheduleId,
        IEnumerable<PayrollRunEmployeeSetupInput> employeeInputs,
        string? userId = null)
    {
        var paySchedule = await _context.PaySchedules
            .FirstOrDefaultAsync(p => p.PayScheduleId == payScheduleId && p.CompanyId == companyId);

        if (paySchedule == null)
        {
            throw new InvalidOperationException("Selected pay schedule was not found for the selected company.");
        }

        var inputs = employeeInputs.ToList();
        if (!inputs.Any())
        {
            throw new InvalidOperationException("Select at least one employee.");
        }

        var employeeIds = inputs.Select(input => input.EmployeeId).Distinct().ToList();
        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId && employeeIds.Contains(e.EmployeeId))
            .ToListAsync();

        if (employees.Count != employeeIds.Count)
        {
            throw new InvalidOperationException("One or more selected employees were not found for the selected company.");
        }

        var payrollRun = new PayrollRun
        {
            CompanyId = companyId,
            PayScheduleId = payScheduleId,
            PayPeriodStart = paySchedule.NextPeriodStartDate,
            PayPeriodEnd = paySchedule.NextPeriodEndDate,
            PayDate = paySchedule.NextPayDate,
            Status = PayrollStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        foreach (var input in inputs)
        {
            var payrollEmployee = new PayrollRunEmployee
            {
                PayrollRunId = payrollRun.PayrollRunId,
                EmployeeId = input.EmployeeId,
                GrossPay = 0m,
                TotalDeductions = 0m,
                TotalTaxes = 0m,
                NetPay = 0m
            };

            if (input.RegularHours > 0)
            {
                payrollEmployee.EarningLines.Add(new EarningLine
                {
                    Description = "Regular Hours",
                    Hours = input.RegularHours,
                    Amount = 0m
                });
            }

            if (input.OvertimeHours > 0)
            {
                payrollEmployee.EarningLines.Add(new EarningLine
                {
                    Description = "Overtime hours placeholder",
                    Hours = input.OvertimeHours,
                    Amount = 0m
                });
            }

            if (input.ManualGrossPayAdjustment > 0)
            {
                payrollEmployee.EarningLines.Add(new EarningLine
                {
                    Description = "Manual gross pay adjustment",
                    Amount = input.ManualGrossPayAdjustment
                });
            }

            if (!string.IsNullOrWhiteSpace(input.Notes))
            {
                payrollEmployee.EarningLines.Add(new EarningLine
                {
                    Description = $"Notes: {input.Notes}",
                    Amount = 0m
                });
            }

            _context.PayrollRunEmployees.Add(payrollEmployee);
        }

        _context.AuditLogEntries.Add(new AuditLogEntry
        {
            PayrollRunId = payrollRun.PayrollRunId,
            EventType = "PayrollRunDraftCreated",
            Description = $"Draft payroll run {payrollRun.PayrollRunId} created for {inputs.Count} employee(s).",
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return payrollRun;
    }

    /// <summary>
    /// Updates payroll run status with validation.
    /// </summary>
    public async Task<bool> UpdatePayrollStatusAsync(int payrollRunId, PayrollStatus newStatus, string? userId = null)
    {
        var payrollRun = await _context.PayrollRuns.FindAsync(payrollRunId);
        if (payrollRun == null) return false;

        // Validate status transition
        if (!IsValidStatusTransition(payrollRun.Status, newStatus))
        {
            throw new InvalidOperationException($"Invalid status transition from {payrollRun.Status} to {newStatus}");
        }

        // Lock after approval
        if (newStatus == PayrollStatus.Approved && payrollRun.Status != PayrollStatus.Approved)
        {
            payrollRun.ApprovedByUserId = userId;
            payrollRun.ApprovedAt = DateTime.UtcNow;
        }

        payrollRun.Status = newStatus;
        _context.PayrollRuns.Update(payrollRun);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Checks if a status transition is valid.
    /// </summary>
    private bool IsValidStatusTransition(PayrollStatus currentStatus, PayrollStatus newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (PayrollStatus.Draft, PayrollStatus.Calculated) => true,
            (PayrollStatus.Calculated, PayrollStatus.Approved) => true,
            (PayrollStatus.Approved, PayrollStatus.Submitted) => true,
            (PayrollStatus.Submitted, PayrollStatus.Completed) => true,
            (PayrollStatus.Draft, PayrollStatus.Voided) => true,
            (PayrollStatus.Calculated, PayrollStatus.Voided) => true,
            (PayrollStatus.Approved, PayrollStatus.Voided) => true,
            (PayrollStatus.Submitted, PayrollStatus.Voided) => true,
            _ => false
        };
    }

    /// <summary>
    /// Checks if a payroll run is locked (approved or later, but not voided).
    /// </summary>
    public bool IsPayrollRunLocked(PayrollRun payrollRun)
    {
        return payrollRun.Status >= PayrollStatus.Approved && payrollRun.Status != PayrollStatus.Voided;
    }

    /// <summary>
    /// Updates payroll run totals.
    /// </summary>
    public async Task<bool> UpdatePayrollTotalsAsync(int payrollRunId)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PayrollRunEmployees)
            .Include(p => p.EmployerTaxLines)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null) return false;

        payrollRun.TotalGrossPay = payrollRun.PayrollRunEmployees.Sum(e => e.GrossPay);
        payrollRun.TotalEmployeeTaxes = payrollRun.PayrollRunEmployees.Sum(e => e.TotalTaxes);
        payrollRun.TotalEmployerTaxes = payrollRun.EmployerTaxLines.Sum(t => t.Amount);
        payrollRun.TotalDeductions = payrollRun.PayrollRunEmployees.Sum(e => e.TotalDeductions);
        payrollRun.TotalNetPay = payrollRun.PayrollRunEmployees.Sum(e => e.NetPay);

        _context.PayrollRuns.Update(payrollRun);
        return await _context.SaveChangesAsync() > 0;
    }
}
