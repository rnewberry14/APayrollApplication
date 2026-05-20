using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Service responsible for approving payroll runs with required validations and audit logging.
/// </summary>
public class PayrollApprovalService
{
    private readonly PayrollDbContext _context;
    private readonly PayrollService _payrollService;

    public PayrollApprovalService(PayrollDbContext context, PayrollService payrollService)
    {
        _context = context;
        _payrollService = payrollService;
    }

    /// <summary>
    /// Approves a payroll run after validating status, blocking errors, and bank verification for direct deposit employees.
    /// </summary>
    public async Task<bool> ApprovePayrollRunAsync(int payrollRunId, string performedByUserId)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.Employee)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.NetPayLines)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");
        }

        // Must be calculated before approval
        if (payrollRun.Status != PayrollStatus.Calculated)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} must be in Calculated status to approve. Current status: {payrollRun.Status}.");
        }

        // Blocking errors: no employees or zero gross
        if (!payrollRun.PayrollRunEmployees.Any())
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} has no employees and cannot be approved.");
        }

        if (payrollRun.TotalGrossPay <= 0)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} has zero total gross pay and cannot be approved.");
        }

        // Validate direct deposit employees have verified bank accounts
        var directDepositEmployees = payrollRun.PayrollRunEmployees
            .Where(pre => pre.NetPayLines.Any(n => n.PaymentMethod == "DirectDeposit"))
            .Select(pre => pre.Employee)
            .Where(e => e != null)
            .Select(e => e!)
            .ToList();

        if (directDepositEmployees.Any())
        {
            var employeeIds = directDepositEmployees.Select(e => e.EmployeeId).ToList();
            var bankAccounts = await _context.EmployeeBankAccounts
                .Where(b => employeeIds.Contains(b.EmployeeId) && b.IsActive)
                .ToListAsync();

            var notVerified = new List<int>();
            foreach (var emp in directDepositEmployees)
            {
                var hasVerified = bankAccounts.Any(b => b.EmployeeId == emp.EmployeeId && b.VerificationStatus == VerificationStatus.Verified && b.IsActive);
                if (!hasVerified)
                {
                    notVerified.Add(emp.EmployeeId);
                }
            }

            if (notVerified.Any())
            {
                throw new InvalidOperationException($"Direct deposit employees missing verified bank accounts: {string.Join(',', notVerified)}");
            }
        }

        // Create audit log entry
        _context.AuditLogEntries.Add(new AuditLogEntry
        {
            PayrollRunId = payrollRunId,
            EventType = "PayrollRunApproved",
            Description = $"Payroll run {payrollRunId} approved by {performedByUserId}.",
            CreatedByUserId = performedByUserId,
            CreatedAt = DateTime.UtcNow
        });

        // Persist audit log first
        await _context.SaveChangesAsync();

        // Update status using existing PayrollService for consistent transitions and approved metadata
        var result = await _payrollService.UpdatePayrollStatusAsync(payrollRunId, PayrollStatus.Approved, performedByUserId);

        return result;
    }
}
