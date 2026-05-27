using ClearPathPayroll.Domain;

namespace ClearPathPayroll.Services;

public class QuickBooksImportTemplateCatalog
{
    public IReadOnlyList<QuickBooksImportTemplate> GetTemplates(QuickBooksImportSource source)
    {
        return source == QuickBooksImportSource.Desktop ? DesktopTemplates : OnlineTemplates;
    }

    public QuickBooksImportTemplate GetTemplate(QuickBooksImportSource source, string templateCode)
    {
        return GetTemplates(source).FirstOrDefault(template => string.Equals(template.TemplateCode, templateCode, StringComparison.OrdinalIgnoreCase))
            ?? GetTemplates(source).First();
    }

    public List<ImportMappingInput> BuildMappings(QuickBooksImportTemplate template, IEnumerable<string> sourceColumns)
    {
        return sourceColumns.Select(column =>
        {
            var target = template.Fields.FirstOrDefault(field => Matches(field, column));
            return new ImportMappingInput
            {
                SourceColumn = column,
                TargetField = target?.TargetField ?? string.Empty,
                IsRequired = target?.IsRequired ?? false
            };
        }).ToList();
    }

    public QuickBooksTemplateValidationResult ValidateMappings(
        QuickBooksImportTemplate template,
        IEnumerable<ImportMappingInput> mappings)
    {
        var mappedTargets = new HashSet<string>(
            mappings.Where(mapping => !string.IsNullOrWhiteSpace(mapping.TargetField)).Select(mapping => mapping.TargetField),
            StringComparer.OrdinalIgnoreCase);

        var missingRequired = template.Fields
            .Where(field => field.IsRequired && !mappedTargets.Contains(field.TargetField))
            .Select(field => field.TargetField)
            .ToList();

        return new QuickBooksTemplateValidationResult
        {
            IsValid = missingRequired.Count == 0,
            MissingRequiredFields = missingRequired
        };
    }

    private static bool Matches(QuickBooksImportField field, string sourceColumn)
    {
        return field.Aliases.Any(alias => Normalize(alias) == Normalize(sourceColumn));
    }

    private static string Normalize(string value)
    {
        return value.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(".", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim()
            .ToUpperInvariant();
    }

    private static readonly IReadOnlyList<QuickBooksImportTemplate> DesktopTemplates = new[]
    {
        Template(
            QuickBooksImportSource.Desktop,
            "desktop-employee-list",
            "Employee list",
            ImportType.EmployeesOnly,
            Field("Employee.FullName", true, "Employee", "Employee Name", "Name", "Full Name"),
            Field("Employee.FirstName", false, "First Name", "First"),
            Field("Employee.LastName", false, "Last Name", "Last"),
            Field("Employee.EmployeeNumber", false, "Employee No.", "Employee Number", "Emp No"),
            Field("Employee.SSNLast4", false, "SSN Last 4", "Last 4 SSN"),
            Field("Employee.PayType", false, "Pay Type", "Payroll Type"),
            Field("Employee.HourlyRate", false, "Hourly Rate", "Rate")),
        Template(
            QuickBooksImportSource.Desktop,
            "desktop-payroll-summary",
            "Payroll summary",
            ImportType.ChecksOnly,
            Field("Check.EmployeeIdentifier", true, "Employee", "Employee Name", "Name"),
            Field("Check.PayDate", true, "Pay Date", "Check Date"),
            Field("Check.GrossPay", true, "Gross Pay", "Gross Wages"),
            Field("Check.EmployeeTaxes", false, "Employee Taxes", "Taxes Withheld"),
            Field("Check.Deductions", false, "Deductions"),
            Field("Check.NetPay", true, "Net Pay", "Net Check")),
        Template(
            QuickBooksImportSource.Desktop,
            "desktop-payroll-item-detail",
            "Payroll item detail",
            ImportType.ChecksOnly,
            Field("Check.EmployeeIdentifier", true, "Employee", "Name"),
            Field("Check.PayDate", true, "Date", "Pay Date"),
            Field("Check.ItemName", true, "Payroll Item", "Item"),
            Field("Check.Amount", true, "Amount"),
            Field("Check.Hours", false, "Hours")),
        Template(
            QuickBooksImportSource.Desktop,
            "desktop-paycheck-detail",
            "Paycheck detail",
            ImportType.ChecksOnly,
            Field("Check.EmployeeIdentifier", true, "Employee", "Name"),
            Field("Check.PayDate", true, "Pay Date", "Check Date"),
            Field("Check.CheckNumber", false, "Check No.", "Check Number", "Num"),
            Field("Check.GrossPay", true, "Gross Pay"),
            Field("Check.NetPay", true, "Net Pay")),
        Template(
            QuickBooksImportSource.Desktop,
            "desktop-tax-liability-payment",
            "Tax liability/payment report placeholder",
            ImportType.TaxDepositsOnly,
            Field("TaxDeposit.Agency", true, "Agency", "Tax Agency"),
            Field("TaxDeposit.TaxType", true, "Tax Type", "Payroll Tax"),
            Field("TaxDeposit.Amount", true, "Amount", "Payment Amount"),
            Field("TaxDeposit.DepositDate", true, "Payment Date", "Deposit Date"),
            Field("TaxDeposit.ConfirmationNumber", false, "Confirmation", "Confirmation Number"))
    };

    private static readonly IReadOnlyList<QuickBooksImportTemplate> OnlineTemplates = new[]
    {
        Template(
            QuickBooksImportSource.Online,
            "qbo-employee-list",
            "Employee list",
            ImportType.EmployeesOnly,
            Field("Employee.DisplayName", true, "Employee", "Display Name", "Name"),
            Field("Employee.FirstName", false, "First name", "First Name"),
            Field("Employee.LastName", false, "Last name", "Last Name"),
            Field("Employee.EmployeeNumber", false, "Employee ID", "Employee Number"),
            Field("Employee.Email", false, "Email", "Email Address"),
            Field("Employee.Phone", false, "Phone", "Phone Number")),
        Template(
            QuickBooksImportSource.Online,
            "qbo-payroll-summary",
            "Payroll summary",
            ImportType.ChecksOnly,
            Field("Check.EmployeeIdentifier", true, "Employee", "Worker", "Name"),
            Field("Check.PayDate", true, "Pay date", "Pay Date"),
            Field("Check.GrossPay", true, "Gross pay", "Gross Pay"),
            Field("Check.EmployeeTaxes", false, "Employee taxes", "Taxes"),
            Field("Check.NetPay", true, "Net pay", "Net Pay")),
        Template(
            QuickBooksImportSource.Online,
            "qbo-payroll-details",
            "Payroll details",
            ImportType.ChecksOnly,
            Field("Check.EmployeeIdentifier", true, "Employee", "Worker"),
            Field("Check.PayDate", true, "Pay date"),
            Field("Check.ItemName", true, "Pay type", "Payroll item"),
            Field("Check.Hours", false, "Hours"),
            Field("Check.Amount", true, "Amount")),
        Template(
            QuickBooksImportSource.Online,
            "qbo-time-activities",
            "Time activities placeholder",
            ImportType.ChecksOnly,
            Field("Time.EmployeeIdentifier", true, "Name", "Employee"),
            Field("Time.Date", true, "Date"),
            Field("Time.Hours", true, "Hours", "Duration"),
            Field("Time.Service", false, "Service", "Activity")),
        Template(
            QuickBooksImportSource.Online,
            "qbo-tax-payments",
            "Tax payments placeholder",
            ImportType.TaxDepositsOnly,
            Field("TaxDeposit.Agency", true, "Agency", "Tax agency"),
            Field("TaxDeposit.TaxType", true, "Tax type", "Tax"),
            Field("TaxDeposit.Amount", true, "Amount", "Payment amount"),
            Field("TaxDeposit.DepositDate", true, "Payment date", "Date"),
            Field("TaxDeposit.ConfirmationNumber", false, "Confirmation number", "Reference no."))
    };

    private static QuickBooksImportTemplate Template(
        QuickBooksImportSource source,
        string code,
        string name,
        ImportType importType,
        params QuickBooksImportField[] fields)
    {
        return new QuickBooksImportTemplate
        {
            Source = source,
            TemplateCode = code,
            TemplateName = name,
            ImportType = importType,
            Fields = fields.ToList()
        };
    }

    private static QuickBooksImportField Field(string targetField, bool required, params string[] aliases)
    {
        return new QuickBooksImportField
        {
            TargetField = targetField,
            IsRequired = required,
            Aliases = aliases.Prepend(targetField).ToList()
        };
    }
}

public enum QuickBooksImportSource
{
    Desktop,
    Online
}

public class QuickBooksImportTemplate
{
    public QuickBooksImportSource Source { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public ImportType ImportType { get; set; }

    public List<QuickBooksImportField> Fields { get; set; } = new();
}

public class QuickBooksImportField
{
    public string TargetField { get; set; } = string.Empty;

    public bool IsRequired { get; set; }

    public List<string> Aliases { get; set; } = new();
}

public class QuickBooksTemplateValidationResult
{
    public bool IsValid { get; set; }

    public List<string> MissingRequiredFields { get; set; } = new();
}
