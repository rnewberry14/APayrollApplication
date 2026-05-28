namespace ClearPathPayroll.Services;

public static class FilingFrequencyOptions
{
    public static IReadOnlyList<string> Values { get; } = new[]
    {
        "Not Set",
        "Monthly",
        "Semiweekly",
        "Quarterly",
        "Annual",
        "Next-Day",
        "Other / User Defined"
    };
}
