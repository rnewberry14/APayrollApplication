using System.Text;

namespace ClearPathPayroll.Services;

public static class ImportFieldAliasMatcher
{
    private static readonly Dictionary<string, string[]> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["FirstName"] = new[] { "FirstName", "First Name", "first_name", "Employee First", "Employee First Name" },
        ["LastName"] = new[] { "LastName", "Last Name", "last_name", "Employee Last", "Employee Last Name" },
        ["FullName"] = new[] { "FullName", "Full Name", "Employee", "Employee Name", "Name", "Worker Name" },
        ["SSNLast4"] = new[] { "SSNLast4", "SSN Last 4", "SSN Last Four", "Last4SSN", "Employee SSN Last 4" },
        ["DateOfBirth"] = new[] { "DateOfBirth", "Date of Birth", "DOB", "Birth Date" },
        ["HireDate"] = new[] { "HireDate", "Hire Date", "Start Date" },
        ["HourlyRate"] = new[] { "HourlyRate", "Hourly Rate", "Rate", "Pay Rate" },
        ["AnnualSalary"] = new[] { "AnnualSalary", "Annual Salary", "Salary", "Base Salary" },
        ["EmployeeNumber"] = new[] { "EmployeeNumber", "Employee Number", "Employee ID", "EmployeeIdentifier" },
        ["PayType"] = new[] { "PayType", "Pay Type", "Compensation Type" },
        ["CompanyIdentifier"] = new[] { "CompanyIdentifier", "Company Identifier", "Company", "Employer", "EmployerName" },
        ["EmployeeIdentifier"] = new[] { "EmployeeIdentifier", "Employee Identifier", "Employee ID", "EmployeeNumber" },
        ["PayDate"] = new[] { "PayDate", "Pay Date", "Check Date" },
        ["PayPeriodStart"] = new[] { "PayPeriodStart", "Pay Period Start", "Period Start" },
        ["PayPeriodEnd"] = new[] { "PayPeriodEnd", "Pay Period End", "Period End" },
        ["GrossPay"] = new[] { "GrossPay", "Gross Pay", "Gross" },
        ["NetPay"] = new[] { "NetPay", "Net Pay", "Net" },
        ["RegularHours"] = new[] { "RegularHours", "Regular Hours", "Reg Hours" },
        ["OvertimeHours"] = new[] { "OvertimeHours", "Overtime Hours", "OT Hours" },
        ["DepositDate"] = new[] { "DepositDate", "Deposit Date", "Payment Date" },
        ["TaxYear"] = new[] { "TaxYear", "Tax Year", "Year" },
        ["EmployerEIN"] = new[] { "EmployerEIN", "Employer EIN", "EIN" },
        ["EmployeeSSNLast4"] = new[] { "EmployeeSSNLast4", "Employee SSN Last 4", "SSN Last 4" }
    };

    public static string SuggestTargetField(string sourceColumn, IEnumerable<string> targetFields)
    {
        var targets = targetFields.ToList();
        var normalizedSource = Normalize(sourceColumn);

        var exact = targets.FirstOrDefault(target => Normalize(target) == normalizedSource);
        if (!string.IsNullOrWhiteSpace(exact))
        {
            return exact;
        }

        foreach (var target in targets)
        {
            var shortTarget = target.Contains('.') ? target.Split('.').Last() : target;
            var aliases = Aliases.TryGetValue(shortTarget, out var configuredAliases)
                ? configuredAliases
                : new[] { shortTarget };

            if (aliases.Any(alias => Normalize(alias) == normalizedSource))
            {
                return target;
            }
        }

        return string.Empty;
    }

    public static int CountUnmappedRequiredFields(IEnumerable<string> requiredTargetFields, IEnumerable<ImportMappingInput> mappings)
    {
        var mappedTargets = mappings
            .Where(mapping => !string.IsNullOrWhiteSpace(mapping.TargetField))
            .Select(mapping => mapping.TargetField.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return requiredTargetFields.Count(required => !mappedTargets.Contains(required));
    }

    public static string Normalize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToUpperInvariant(character));
            }
        }

        return builder.ToString();
    }

    public static string SanitizeErrorMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "Import validation found an issue that requires review.";
        }

        return message.Length > 180 ? $"{message[..180]}..." : message;
    }
}
