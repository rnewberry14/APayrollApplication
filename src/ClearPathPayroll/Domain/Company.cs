using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents a company in the payroll system.
/// </summary>
[Table("Companies")]
public class Company : IValidatableObject
{
    /// <summary>
    /// Unique identifier for the company.
    /// </summary>
    [Key]
    public int CompanyId { get; set; }

    /// <summary>
    /// Legal name of the company.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LegalName { get; set; } = string.Empty;

    /// <summary>
    /// Doing Business As name.
    /// </summary>
    [StringLength(100)]
    public string? DBAName { get; set; }

    /// <summary>
    /// Federal Employer Identification Number. SENSITIVE: Do not expose in UI casually.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string FEIN { get; set; } = string.Empty; // SENSITIVE

    /// <summary>
    /// Primary address of the company.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string PrimaryAddress { get; set; } = string.Empty;

    /// <summary>
    /// City of the company.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State of the company.
    /// </summary>
    [Required]
    [StringLength(2)]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Zip code of the company.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string ZipCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string? County { get; set; }

    [StringLength(100)]
    public string? LocalTaxLocalityPlaceholder { get; set; }

    /// <summary>
    /// Phone number of the company.
    /// </summary>
    [Phone]
    [StringLength(15)]
    public string? Phone { get; set; }

    /// <summary>
    /// Email address of the company.
    /// </summary>
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    /// <summary>
    /// Name of the payroll contact person.
    /// </summary>
    [StringLength(100)]
    public string? PayrollContactName { get; set; }

    /// <summary>
    /// Date and time when the company was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the company was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates if the company is active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    [Range(0, 100)]
    public decimal? FutaRatePlaceholder { get; set; }

    [StringLength(2)]
    public string? SutaState { get; set; }

    [StringLength(100)]
    public string? SutaEmployerAccountNumberPlaceholder { get; set; }

    [Range(0, 100)]
    public decimal? SutaRate { get; set; }

    [StringLength(100)]
    public string? StateWithholdingAccountNumberPlaceholder { get; set; }

    [StringLength(100)]
    public string? LocalTaxAccountNumberPlaceholder { get; set; }

    [StringLength(100)]
    public string? DepositSchedulePlaceholder { get; set; }

    [StringLength(100)]
    public string? FilingFrequencyPlaceholder { get; set; }

    public DateTime? EmployerTaxEffectiveDate { get; set; }

    [StringLength(2000)]
    public string? EmployerTaxNotes { get; set; }

    public ICollection<PayrollItem> PayrollItems { get; set; } = new List<PayrollItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidatePlaceholderAccount(nameof(SutaEmployerAccountNumberPlaceholder), SutaEmployerAccountNumberPlaceholder))
        {
            yield return result;
        }

        foreach (var result in ValidatePlaceholderAccount(nameof(StateWithholdingAccountNumberPlaceholder), StateWithholdingAccountNumberPlaceholder))
        {
            yield return result;
        }

        foreach (var result in ValidatePlaceholderAccount(nameof(LocalTaxAccountNumberPlaceholder), LocalTaxAccountNumberPlaceholder))
        {
            yield return result;
        }
    }

    private static IEnumerable<ValidationResult> ValidatePlaceholderAccount(string memberName, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Length >= 5 && value.All(char.IsDigit))
        {
            yield return new ValidationResult(
                "Account number fields require a placeholder or masked value, not a plain account number.",
                new[] { memberName });
        }
    }
}
