using ClearPathPayroll.Domain;

namespace ClearPathPayroll.Components.Pages;

/// <summary>
/// Helper class for minimum wage validation and tipped employee handling.
/// </summary>
public static class MinimumWageValidator
{
    /// <summary>
    /// Federal minimum wage for regular employees.
    /// </summary>
    public const decimal FederalMinimumWage = 7.25m;

    /// <summary>
    /// Federal minimum wage for tipped employees.
    /// </summary>
    public const decimal FederalTippedMinimumWage = 2.13m;

    /// <summary>
    /// State minimum wages (as of May 2026).
    /// Update this mapping as state laws change.
    /// </summary>
    private static readonly Dictionary<string, decimal> StateMinimumWages = new()
    {
        // States with higher than federal minimum
        { "AL", 7.25m },  // Alabama - Federal
        { "AK", 11.73m }, // Alaska
        { "AZ", 15.00m }, // Arizona
        { "AR", 11.00m }, // Arkansas
        { "CA", 16.50m }, // California
        { "CO", 14.42m }, // Colorado
        { "CT", 15.69m }, // Connecticut
        { "DE", 13.25m }, // Delaware
        { "FL", 13.00m }, // Florida
        { "GA", 7.25m },  // Georgia - Federal
        { "HI", 14.00m }, // Hawaii
        { "ID", 10.99m }, // Idaho
        { "IL", 14.00m }, // Illinois
        { "IN", 7.25m },  // Indiana - Federal
        { "IA", 11.00m }, // Iowa
        { "KS", 10.30m }, // Kansas
        { "KY", 10.30m }, // Kentucky
        { "LA", 7.25m },  // Louisiana - Federal
        { "ME", 14.15m }, // Maine
        { "MD", 15.13m }, // Maryland
        { "MA", 15.00m }, // Massachusetts
        { "MI", 12.00m }, // Michigan
        { "MN", 12.85m }, // Minnesota
        { "MS", 7.25m },  // Mississippi - Federal
        { "MO", 12.30m }, // Missouri
        { "MT", 12.30m }, // Montana
        { "NE", 14.00m }, // Nebraska
        { "NV", 12.00m }, // Nevada (regular rate, $11.00 for employees not provided health insurance)
        { "NH", 7.25m },  // New Hampshire - Federal
        { "NJ", 15.13m }, // New Jersey
        { "NM", 12.00m }, // New Mexico
        { "NY", 15.00m }, // New York
        { "NC", 7.25m },  // North Carolina - Federal
        { "ND", 12.30m }, // North Dakota
        { "OH", 11.00m }, // Ohio
        { "OK", 7.25m },  // Oklahoma - Federal
        { "OR", 15.45m }, // Oregon
        { "PA", 7.25m },  // Pennsylvania - Federal
        { "RI", 15.00m }, // Rhode Island
        { "SC", 7.25m },  // South Carolina - Federal
        { "SD", 14.00m }, // South Dakota
        { "TN", 7.25m },  // Tennessee - Federal
        { "TX", 7.25m },  // Texas - Federal
        { "UT", 7.25m },  // Utah - Federal
        { "VT", 14.67m }, // Vermont
        { "VA", 12.00m }, // Virginia
        { "WA", 16.28m }, // Washington
        { "WV", 8.75m },  // West Virginia
        { "WI", 10.86m }, // Wisconsin
        { "WY", 10.63m }  // Wyoming
    };

    /// <summary>
    /// Gets the minimum wage for a given state.
    /// </summary>
    public static decimal GetStateMinimumWage(string? state)
    {
        if (string.IsNullOrWhiteSpace(state) || state.Length != 2)
            return FederalMinimumWage;

        var upperState = state.ToUpper();
        return StateMinimumWages.TryGetValue(upperState, out var wage) 
            ? wage 
            : FederalMinimumWage;
    }

    /// <summary>
    /// Detects if an employee is a tipped employee based on earning line descriptions.
    /// </summary>
    public static bool IsTippedEmployee(IEnumerable<EarningLine>? earningLines)
    {
        if (earningLines == null || !earningLines.Any())
            return false;

        var tipKeywords = new[] { "tip", "gratuity", "service charge", "credit card tip", "cash tip" };
        
        return earningLines.Any(line =>
            tipKeywords.Any(keyword =>
                line.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false
            )
        );
    }

    /// <summary>
    /// Gets the total tip amount from earning lines.
    /// </summary>
    public static decimal GetTipAmount(IEnumerable<EarningLine>? earningLines)
    {
        if (earningLines == null)
            return 0m;

        var tipKeywords = new[] { "tip", "gratuity", "service charge", "credit card tip", "cash tip" };

        return earningLines
            .Where(line =>
                tipKeywords.Any(keyword =>
                    line.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false
                )
            )
            .Sum(line => line.Amount);
    }

    /// <summary>
    /// Gets the base wage (excluding tips) from earning lines.
    /// </summary>
    public static decimal GetBaseWageAmount(IEnumerable<EarningLine>? earningLines)
    {
        if (earningLines == null)
            return 0m;

        var tipKeywords = new[] { "tip", "gratuity", "service charge", "credit card tip", "cash tip" };

        return earningLines
            .Where(line =>
                !tipKeywords.Any(keyword =>
                    line.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false
                )
            )
            .Sum(line => line.Amount);
    }

    /// <summary>
    /// Gets the total hours worked from earning lines.
    /// </summary>
    public static decimal GetTotalHours(IEnumerable<EarningLine>? earningLines)
    {
        if (earningLines == null)
            return 0m;

        return earningLines
            .Where(line => line.Hours.HasValue && line.Hours.Value > 0)
            .Sum(line => line.Hours ?? 0m);
    }

    /// <summary>
    /// Validates minimum wage for an employee and returns warning if applicable.
    /// </summary>
    /// <returns>
    /// Warning message if minimum wage is not met, null if no warning needed.
    /// </returns>
    public static string? ValidateMinimumWage(
        string employeeName,
        string? state,
        PayType payType,
        decimal grossPay,
        decimal? totalHours,
        IEnumerable<EarningLine>? earningLines)
    {
        // Only validate hourly employees
        if (payType != PayType.Hourly)
            return null;

        // Need hours to validate minimum wage
        if (!totalHours.HasValue || totalHours.Value <= 0)
            return null;

        // Get state minimum wage
        var stateMinimum = GetStateMinimumWage(state);

        // Calculate effective hourly rate from gross pay
        var effectiveHourlyRate = grossPay / totalHours.Value;

        // Check if employee has tips
        var isTipped = IsTippedEmployee(earningLines);
        var tipAmount = GetTipAmount(earningLines);
        var baseWage = GetBaseWageAmount(earningLines);

        if (isTipped && tipAmount > 0)
        {
            // For tipped employees:
            // - Base wage can be lower (federal minimum is $2.13)
            // - Tips + base wage must equal at least state minimum
            // - But warn if base wage is below the allowable tipped rate

            var federalTippedMinimum = FederalTippedMinimumWage;
            var baseWagePerHour = baseWage / totalHours.Value;

            // Check if combined base + tips meets minimum wage requirement
            if (grossPay < (stateMinimum * totalHours.Value))
            {
                return $"{employeeName}: Base wage + tips (${grossPay:F2}) falls short of " +
                       $"{state?.ToUpper()} minimum wage requirement of ${stateMinimum * totalHours.Value:F2} " +
                       $"({totalHours:F2} hours × ${stateMinimum:F2}/hr).";
            }

            // Warn if base wage is unreasonably low (below federal tipped minimum)
            if (baseWagePerHour < federalTippedMinimum)
            {
                return $"{employeeName}: Base wage of ${baseWagePerHour:F2}/hr falls below " +
                       $"federal tipped employee minimum of ${federalTippedMinimum:F2}/hr. " +
                       $"Tips (${tipAmount:F2}) must make up the difference to reach state minimum.";
            }
        }
        else
        {
            // For non-tipped employees: gross pay / hours must meet minimum wage
            if (effectiveHourlyRate < stateMinimum)
            {
                return $"{employeeName}: Effective hourly rate (${effectiveHourlyRate:F2}/hr) " +
                       $"is below {state?.ToUpper()} minimum wage of ${stateMinimum:F2}/hr.";
            }
        }

        return null;
    }
}
