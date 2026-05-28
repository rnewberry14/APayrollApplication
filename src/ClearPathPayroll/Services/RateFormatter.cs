using System.Globalization;

namespace ClearPathPayroll.Services;

public static class RateFormatter
{
    public static bool TryParsePercentRate(string? input, out decimal? rate, out string? errorMessage)
    {
        rate = null;
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        var cleaned = input.Trim().TrimEnd('%');
        if (!decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
        {
            errorMessage = "Rate must be a number.";
            return false;
        }

        if (parsed < 0 || parsed > 100)
        {
            errorMessage = "Rate must be between 0 and 100.";
            return false;
        }

        rate = Math.Round(parsed, 4, MidpointRounding.AwayFromZero);
        return true;
    }

    public static string FormatPercentRate(decimal? rate)
    {
        return rate.HasValue ? rate.Value.ToString("0.0000", CultureInfo.InvariantCulture) : string.Empty;
    }
}
