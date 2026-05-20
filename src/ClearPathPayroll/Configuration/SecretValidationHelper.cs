namespace ClearPathPayroll.Configuration;

public static class SecretValidationHelper
{
    public static bool IsProductionValueConfigured(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        return !normalized.StartsWith("ENTER_", StringComparison.OrdinalIgnoreCase)
            && !normalized.Contains("placeholder", StringComparison.OrdinalIgnoreCase)
            && !normalized.Contains("replace", StringComparison.OrdinalIgnoreCase);
    }
}
