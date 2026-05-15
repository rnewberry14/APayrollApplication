using ClearPathPayroll.Domain;

namespace ClearPathPayroll.Services;

/// <summary>
/// Result of gross pay calculation.
/// </summary>
public class GrossPayCalculationResult
{
    /// <summary>
    /// Regular hours worked.
    /// </summary>
    public decimal RegularHours { get; set; }

    /// <summary>
    /// Overtime hours worked (placeholder).
    /// </summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>
    /// Pay for regular hours.
    /// </summary>
    public decimal RegularPay { get; set; }

    /// <summary>
    /// Pay for overtime hours (placeholder at 1.5x rate).
    /// </summary>
    public decimal OvertimePay { get; set; }

    /// <summary>
    /// Total gross pay.
    /// </summary>
    public decimal TotalGrossPay { get; set; }

    /// <summary>
    /// Pay frequency used for calculation.
    /// </summary>
    public PayFrequency PayFrequency { get; set; }

    /// <summary>
    /// Pay type (Hourly or Salary).
    /// </summary>
    public PayType PayType { get; set; }
}

/// <summary>
/// Service for calculating gross pay.
/// </summary>
public class GrossPayCalculationService
{
    /// <summary>
    /// Calculates gross pay for an employee based on pay schedule.
    /// </summary>
    public GrossPayCalculationResult CalculateGrossPay(Employee employee, PaySchedule paySchedule, decimal? hoursWorked = null)
    {
        var result = new GrossPayCalculationResult
        {
            PayFrequency = paySchedule.Frequency,
            PayType = employee.PayType
        };

        if (employee.PayType == PayType.Salary)
        {
            // Salary calculation
            var periodsPerYear = GetPeriodsPerYear(paySchedule.Frequency);
            result.TotalGrossPay = Math.Round(employee.AnnualSalary.GetValueOrDefault() / periodsPerYear, 2);
            result.RegularPay = result.TotalGrossPay;
            result.RegularHours = 0; // Not applicable for salary
        }
        else if (employee.PayType == PayType.Hourly)
        {
            // Hourly calculation
            var rate = employee.HourlyRate.GetValueOrDefault();
            var hours = hoursWorked.GetValueOrDefault();

            if (hours <= 40)
            {
                // All regular hours
                result.RegularHours = hours;
                result.OvertimeHours = 0;
                result.RegularPay = Math.Round(hours * rate, 2);
                result.OvertimePay = 0;
            }
            else
            {
                // Overtime placeholder: simple 1.5x for hours over 40
                result.RegularHours = 40;
                result.OvertimeHours = hours - 40;
                result.RegularPay = Math.Round(40 * rate, 2);
                result.OvertimePay = Math.Round((hours - 40) * rate * 1.5m, 2);
            }

            result.TotalGrossPay = Math.Round(result.RegularPay + result.OvertimePay, 2);
        }

        return result;
    }

    /// <summary>
    /// Gets the number of pay periods per year for a frequency.
    /// </summary>
    private int GetPeriodsPerYear(PayFrequency frequency)
    {
        return frequency switch
        {
            PayFrequency.Weekly => 52,
            PayFrequency.Biweekly => 26,
            PayFrequency.Semimonthly => 24,
            PayFrequency.Monthly => 12,
            _ => throw new ArgumentException("Invalid pay frequency")
        };
    }
}