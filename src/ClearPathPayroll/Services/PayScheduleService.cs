using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Service for managing pay schedules.
/// </summary>
public class PayScheduleService
{
    private readonly PayrollDbContext _context;

    public PayScheduleService(PayrollDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all active pay schedules for a company.
    /// </summary>
    public async Task<List<PaySchedule>> GetActivePaySchedulesByCompanyAsync(int companyId)
    {
        return await _context.PaySchedules
            .Where(p => p.CompanyId == companyId && p.IsActive)
            .Include(p => p.Company)
            .ToListAsync();
    }

    public async Task<List<PaySchedule>> GetPaySchedulesByCompanyAsync(int companyId)
    {
        return await _context.PaySchedules
            .Where(p => p.CompanyId == companyId)
            .Include(p => p.Company)
            .OrderByDescending(p => p.IsActive)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a pay schedule by ID.
    /// </summary>
    public async Task<PaySchedule?> GetPayScheduleByIdAsync(int id)
    {
        return await _context.PaySchedules
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.PayScheduleId == id);
    }

    /// <summary>
    /// Creates a new pay schedule.
    /// </summary>
    public async Task<PaySchedule> CreatePayScheduleAsync(PaySchedule paySchedule)
    {
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();
        return paySchedule;
    }

    /// <summary>
    /// Updates an existing pay schedule.
    /// </summary>
    public async Task<bool> UpdatePayScheduleAsync(PaySchedule paySchedule)
    {
        _context.PaySchedules.Update(paySchedule);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Calculates and updates the next pay period after a payroll run.
    /// </summary>
    public void CalculateNextPayPeriod(PaySchedule paySchedule)
    {
        switch (paySchedule.Frequency)
        {
            case PayFrequency.Weekly:
                paySchedule.NextPayDate = paySchedule.NextPayDate.AddDays(7);
                paySchedule.NextPeriodStartDate = paySchedule.NextPeriodStartDate.AddDays(7);
                paySchedule.NextPeriodEndDate = paySchedule.NextPeriodEndDate.AddDays(7);
                break;

            case PayFrequency.Biweekly:
                paySchedule.NextPayDate = paySchedule.NextPayDate.AddDays(14);
                paySchedule.NextPeriodStartDate = paySchedule.NextPeriodStartDate.AddDays(14);
                paySchedule.NextPeriodEndDate = paySchedule.NextPeriodEndDate.AddDays(14);
                break;

            case PayFrequency.Semimonthly:
                CalculateNextSemimonthlyPeriod(paySchedule);
                break;

            case PayFrequency.Monthly:
                CalculateNextMonthlyPeriod(paySchedule);
                break;

            default:
                throw new ArgumentException("Invalid pay frequency");
        }
    }

    private void CalculateNextSemimonthlyPeriod(PaySchedule paySchedule)
    {
        // Semimonthly typically pays on 15th and last day of month
        var currentPayDate = paySchedule.NextPayDate;
        var nextPayDate = currentPayDate.AddMonths(1);

        // If current is 15th, next is end of month
        if (currentPayDate.Day == 15)
        {
            nextPayDate = new DateTime(currentPayDate.Year, currentPayDate.Month, DateTime.DaysInMonth(currentPayDate.Year, currentPayDate.Month));
        }
        // If current is end of month, next is 15th of next month
        else if (currentPayDate.Day == DateTime.DaysInMonth(currentPayDate.Year, currentPayDate.Month))
        {
            nextPayDate = new DateTime(currentPayDate.Year, currentPayDate.Month, 15).AddMonths(1);
        }

        paySchedule.NextPayDate = nextPayDate;

        // For simplicity, assume period starts after previous pay date and ends on pay date
        paySchedule.NextPeriodStartDate = paySchedule.NextPeriodEndDate.AddDays(1);
        paySchedule.NextPeriodEndDate = nextPayDate;
    }

    private void CalculateNextMonthlyPeriod(PaySchedule paySchedule)
    {
        var currentPayDate = paySchedule.NextPayDate;
        var nextPayDate = currentPayDate.AddMonths(1);

        // Handle end-of-month: if original was last day, next should be last day of next month
        if (currentPayDate.Day == DateTime.DaysInMonth(currentPayDate.Year, currentPayDate.Month))
        {
            nextPayDate = new DateTime(nextPayDate.Year, nextPayDate.Month, DateTime.DaysInMonth(nextPayDate.Year, nextPayDate.Month));
        }

        paySchedule.NextPayDate = nextPayDate;
        paySchedule.NextPeriodStartDate = paySchedule.NextPeriodEndDate.AddDays(1);
        paySchedule.NextPeriodEndDate = nextPayDate;
    }
}
