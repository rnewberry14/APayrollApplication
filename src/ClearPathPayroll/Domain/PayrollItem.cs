using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

public enum PayrollItemType
{
    Earnings,
    Deductions,
    EmployeeTaxes,
    EmployerTaxes,
    Reimbursements,
    MemoOnlyItems
}

public enum PayrollItemCalculationType
{
    FixedAmount,
    HourlyRate,
    Percentage,
    UserDefinedFormulaPlaceholder,
    ManualEntry
}

[Table("PayrollItems")]
public class PayrollItem : IValidatableObject
{
    [Key]
    public int PayrollItemId { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public Company? Company { get; set; }

    [Required]
    [StringLength(50)]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ItemName { get; set; } = string.Empty;

    public PayrollItemType ItemType { get; set; } = PayrollItemType.Earnings;

    public PayrollItemCalculationType CalculationType { get; set; } = PayrollItemCalculationType.ManualEntry;

    [Range(0, double.MaxValue)]
    public decimal? DefaultAmount { get; set; }

    [Range(0, 100)]
    public decimal? DefaultRate { get; set; }

    public bool IsTaxableFederal { get; set; }

    public bool IsTaxableState { get; set; }

    public bool IsTaxableLocal { get; set; }

    public bool IsSubjectToSocialSecurity { get; set; }

    public bool IsSubjectToMedicare { get; set; }

    public bool IsPretaxDeduction { get; set; }

    public bool IsPosttaxDeduction { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? EffectiveDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EffectiveDate.HasValue && EndDate.Value.Date < EffectiveDate.Value.Date)
        {
            yield return new ValidationResult(
                "End date cannot be before effective date.",
                new[] { nameof(EndDate), nameof(EffectiveDate) });
        }

        if (CalculationType == PayrollItemCalculationType.FixedAmount && DefaultAmount.HasValue && DefaultAmount.Value < 0)
        {
            yield return new ValidationResult("Default amount cannot be negative.", new[] { nameof(DefaultAmount) });
        }

        if (CalculationType == PayrollItemCalculationType.Percentage && DefaultRate.HasValue && (DefaultRate.Value < 0 || DefaultRate.Value > 100))
        {
            yield return new ValidationResult("Default rate must be between 0 and 100.", new[] { nameof(DefaultRate) });
        }
    }
}
