namespace ClearPathPayroll.Services;

public static class PhoneNumberFormatter
{
    public static bool TryFormat(string? input, out string? formatted, out string? errorMessage)
    {
        formatted = null;
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        var digits = new string(input.Where(char.IsDigit).ToArray());
        if (digits.Length != 10)
        {
            errorMessage = "Phone number must contain 10 digits.";
            return false;
        }

        formatted = $"({digits[..3]}) {digits.Substring(3, 3)}-{digits[6..]}";
        return true;
    }
}
