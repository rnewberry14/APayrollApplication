using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace ClearPathPayroll.Domain;

public enum UserDefinedFieldAppliesTo
{
    Company,
    Employee,
    PayrollRun,
    PayrollRunEmployee,
    CheckLine
}

public enum UserDefinedFieldDataType
{
    Text,
    Number,
    Currency,
    Percent,
    Date,
    Boolean,
    List
}

public enum UserDefinedFieldCalculationRole
{
    InformationalOnly,
    Hours,
    Rate,
    Amount,
    DeductionAmount,
    EarningAmount,
    TaxableWageAdjustmentPlaceholder
}

[Table("UserDefinedFieldDefinitions")]
public class UserDefinedFieldDefinition : IValidatableObject
{
    private static readonly Regex SafeCodePattern = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

    [Key]
    public int UserDefinedFieldDefinitionId { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public Company? Company { get; set; }

    [Required]
    [StringLength(100)]
    public string FieldName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FieldCode { get; set; } = string.Empty;

    public UserDefinedFieldAppliesTo AppliesTo { get; set; } = UserDefinedFieldAppliesTo.Employee;

    public UserDefinedFieldDataType DataType { get; set; } = UserDefinedFieldDataType.Text;

    public bool IsRequired { get; set; }

    [StringLength(1000)]
    public string? DefaultValue { get; set; }

    [StringLength(2000)]
    public string? ListOptions { get; set; }

    public bool IncludeInPayrollCalculation { get; set; }

    public UserDefinedFieldCalculationRole CalculationRole { get; set; } = UserDefinedFieldCalculationRole.InformationalOnly;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserDefinedFieldValue> Values { get; set; } = new List<UserDefinedFieldValue>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(FieldCode) && !SafeCodePattern.IsMatch(FieldCode))
        {
            yield return new ValidationResult(
                "Field code can contain letters, numbers, underscores, and hyphens only.",
                new[] { nameof(FieldCode) });
        }

        if (DataType == UserDefinedFieldDataType.List && string.IsNullOrWhiteSpace(ListOptions))
        {
            yield return new ValidationResult(
                "List options are required for list fields.",
                new[] { nameof(ListOptions) });
        }

        foreach (var result in ValidateTextSafety(nameof(FieldName), FieldName))
        {
            yield return result;
        }

        foreach (var result in ValidateTextSafety(nameof(DefaultValue), DefaultValue))
        {
            yield return result;
        }

        foreach (var result in ValidateTextSafety(nameof(ListOptions), ListOptions))
        {
            yield return result;
        }
    }

    private static IEnumerable<ValidationResult> ValidateTextSafety(string memberName, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            yield break;
        }

        var lower = value.ToLowerInvariant();
        if (lower.Contains("<script") || lower.Contains("javascript:") || lower.Contains("{{") || lower.Contains("}}"))
        {
            yield return new ValidationResult(
                "User-defined fields do not allow scripts, templates, or executable expressions.",
                new[] { memberName });
        }
    }
}
