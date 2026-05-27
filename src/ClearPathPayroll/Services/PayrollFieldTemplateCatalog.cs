using ClearPathPayroll.Domain;

namespace ClearPathPayroll.Services;

public sealed record PayrollFieldTemplate(
    string TemplateKey,
    string FieldName,
    string FieldCode,
    UserDefinedFieldAppliesTo AppliesTo,
    UserDefinedFieldDataType DataType,
    UserDefinedFieldCalculationRole CalculationRole,
    bool IncludeInPayrollCalculation,
    string Description);

public class PayrollFieldTemplateCatalog
{
    private static readonly IReadOnlyList<PayrollFieldTemplate> Templates =
    [
        new("department", "Department", "DEPARTMENT", UserDefinedFieldAppliesTo.Employee, UserDefinedFieldDataType.Text, UserDefinedFieldCalculationRole.InformationalOnly, false, "Common optional payroll field template."),
        new("job-costing-code", "Job costing code", "JOB_COSTING_CODE", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Text, UserDefinedFieldCalculationRole.InformationalOnly, false, "Common optional payroll field template."),
        new("workers-comp-class", "Workers comp class", "WORKERS_COMP_CLASS", UserDefinedFieldAppliesTo.Employee, UserDefinedFieldDataType.Text, UserDefinedFieldCalculationRole.InformationalOnly, false, "Common optional payroll field template."),
        new("certified-payroll-classification", "Certified payroll classification", "CERTIFIED_PAYROLL_CLASSIFICATION", UserDefinedFieldAppliesTo.Employee, UserDefinedFieldDataType.Text, UserDefinedFieldCalculationRole.InformationalOnly, false, "Common optional payroll field template."),
        new("union-code-placeholder", "Union code placeholder", "UNION_CODE_PLACEHOLDER", UserDefinedFieldAppliesTo.Employee, UserDefinedFieldDataType.Text, UserDefinedFieldCalculationRole.InformationalOnly, false, "Common optional payroll field template."),
        new("project-code", "Project code", "PROJECT_CODE", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Text, UserDefinedFieldCalculationRole.InformationalOnly, false, "Common optional payroll field template."),
        new("location-code", "Location code", "LOCATION_CODE", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Text, UserDefinedFieldCalculationRole.InformationalOnly, false, "Common optional payroll field template."),
        new("tip-amount", "Tip amount", "TIP_AMOUNT", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.Amount, true, "Common optional payroll field template."),
        new("cash-tips", "Cash tips", "CASH_TIPS", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.Amount, true, "Common optional payroll field template."),
        new("reported-tips", "Reported tips", "REPORTED_TIPS", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.Amount, true, "Common optional payroll field template."),
        new("mileage-reimbursement", "Mileage reimbursement", "MILEAGE_REIMBURSEMENT", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.Amount, true, "Common optional payroll field template."),
        new("pto-hours", "PTO hours", "PTO_HOURS", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Number, UserDefinedFieldCalculationRole.Hours, true, "Common optional payroll field template."),
        new("sick-hours", "Sick hours", "SICK_HOURS", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Number, UserDefinedFieldCalculationRole.Hours, true, "Common optional payroll field template."),
        new("vacation-hours", "Vacation hours", "VACATION_HOURS", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Number, UserDefinedFieldCalculationRole.Hours, true, "Common optional payroll field template."),
        new("bonus-amount", "Bonus amount", "BONUS_AMOUNT", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.EarningAmount, true, "Common optional payroll field template."),
        new("commission-amount", "Commission amount", "COMMISSION_AMOUNT", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.EarningAmount, true, "Common optional payroll field template."),
        new("piecework-units", "Piecework units", "PIECEWORK_UNITS", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Number, UserDefinedFieldCalculationRole.Hours, true, "Common optional payroll field template."),
        new("shift-differential", "Shift differential", "SHIFT_DIFFERENTIAL", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.Rate, true, "Common optional payroll field template."),
        new("garnishment-placeholder", "Garnishment placeholder", "GARNISHMENT_PLACEHOLDER", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.DeductionAmount, true, "Common optional payroll field template."),
        new("child-support-withholding-placeholder", "Child support withholding placeholder", "CHILD_SUPPORT_WITHHOLDING_PLACEHOLDER", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.DeductionAmount, true, "Common optional payroll field template."),
        new("health-insurance-deduction", "Health insurance deduction", "HEALTH_INSURANCE_DEDUCTION", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.DeductionAmount, true, "Common optional payroll field template."),
        new("retirement-deduction", "Retirement deduction", "RETIREMENT_DEDUCTION", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.DeductionAmount, true, "Common optional payroll field template."),
        new("employer-contribution-placeholder", "Employer contribution placeholder", "EMPLOYER_CONTRIBUTION_PLACEHOLDER", UserDefinedFieldAppliesTo.PayrollRunEmployee, UserDefinedFieldDataType.Currency, UserDefinedFieldCalculationRole.Amount, true, "Common optional payroll field template.")
    ];

    public IReadOnlyList<PayrollFieldTemplate> GetTemplates()
    {
        return Templates;
    }

    public PayrollFieldTemplate? FindTemplate(string templateKey)
    {
        return Templates.FirstOrDefault(template => string.Equals(template.TemplateKey, templateKey, StringComparison.OrdinalIgnoreCase));
    }
}
