using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

public enum EmploymentStatus
{
    Active,
    Inactive,
    Terminated
}

public enum PayType
{
    Hourly,
    Salary
}

public enum WorkerType
{
    W2Employee,
    ContractorPlaceholder
}

public enum FederalFilingStatus
{
    NotSpecified,
    SingleOrMarriedFilingSeparately,
    MarriedFilingJointly,
    HeadOfHousehold
}

public enum StateFilingStatus
{
    NotSpecified,
    Single,
    Married,
    HeadOfHousehold,
    Other
}

[Table("Employees")]
public class Employee : IValidatableObject
{
    [Key]
    public int EmployeeId { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(1)]
    public string? MiddleInitial { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Suffix { get; set; }

    [StringLength(50)]
    public string? PreferredName { get; set; }

    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string SSNLast4 { get; set; } = string.Empty;

    [StringLength(256)]
    public string? FullSSNEncryptedPlaceholder { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public DateTime HireDate { get; set; }

    public DateTime? RehireDate { get; set; }

    public DateTime? TerminationDate { get; set; }

    [Required]
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;

    [Required]
    public WorkerType WorkerType { get; set; } = WorkerType.W2Employee;

    [Required]
    public PayType PayType { get; set; }

    [StringLength(100)]
    public string? EmployeeNumber { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    [StringLength(100)]
    public string? JobTitle { get; set; }

    public int? PrimaryWorkLocationId { get; set; }

    public int? WorkLocationId { get; set; }

    public bool IsTippedEmployee { get; set; }

    public bool IsExemptFromOvertime { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(0, double.MaxValue)]
    public decimal? HourlyRate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? AnnualSalary { get; set; }

    [Range(0, 168)]
    public decimal? DefaultRegularHours { get; set; }

    [Range(0, 168)]
    public decimal? DefaultOvertimeHours { get; set; }

    public PayFrequency? DefaultPayFrequency { get; set; }

    [StringLength(50)]
    public string? DefaultEarningCode { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? TippedCashWageRate { get; set; }

    public bool TipCreditAllowedPlaceholder { get; set; }

    public bool MinimumWageWarningEnabled { get; set; } = true;

    [Required]
    [StringLength(200)]
    public string ResidenceAddress { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address1 { get; set; }

    [StringLength(200)]
    public string? Address2 { get; set; }

    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(2)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string ZipCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string? County { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(25)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? EmergencyContactName { get; set; }

    [Phone]
    [StringLength(25)]
    public string? EmergencyContactPhone { get; set; }

    public FederalFilingStatus FilingStatus { get; set; } = FederalFilingStatus.NotSpecified;

    public bool MultipleJobsOrSpouseWorks { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DependentsAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal OtherIncome { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Deductions { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ExtraWithholding { get; set; }

    public bool ExemptFromFederalWithholding { get; set; }

    [Range(2020, 2100)]
    public int? W4Year { get; set; }

    public DateTime? W4SignedDate { get; set; }

    [StringLength(1000)]
    public string? W4Notes { get; set; }

    [StringLength(2)]
    public string? StateTaxState { get; set; }

    public StateFilingStatus StateFilingStatus { get; set; } = StateFilingStatus.NotSpecified;

    [Range(0, 99)]
    public int? StateAllowances { get; set; }

    [Range(0, double.MaxValue)]
    public decimal StateAdditionalWithholding { get; set; }

    public bool StateExemptFromWithholding { get; set; }

    [StringLength(50)]
    public string? StateTaxFormVersion { get; set; }

    public DateTime? StateTaxSignedDate { get; set; }

    [StringLength(1000)]
    public string? StateTaxNotes { get; set; }

    [StringLength(2000)]
    public string? PayrollNotes { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime PayrollProfileCreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PayrollProfileUpdatedAt { get; set; }

    public ICollection<EmployeeBankAccount> BankAccounts { get; set; } = new List<EmployeeBankAccount>();

    public ICollection<EmployeePayrollField> PayrollFields { get; set; } = new List<EmployeePayrollField>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(FullSSNEncryptedPlaceholder))
        {
            yield return new ValidationResult(
                "Only SSN last four may be stored on the employee payroll profile.",
                new[] { nameof(FullSSNEncryptedPlaceholder) });
        }

        if (PayType == PayType.Hourly && (!HourlyRate.HasValue || HourlyRate.Value <= 0))
        {
            yield return new ValidationResult(
                "Hourly employees require an hourly rate greater than zero.",
                new[] { nameof(HourlyRate) });
        }

        if (PayType == PayType.Salary && (!AnnualSalary.HasValue || AnnualSalary.Value <= 0))
        {
            yield return new ValidationResult(
                "Salary employees require an annual salary greater than zero.",
                new[] { nameof(AnnualSalary) });
        }

        if (TerminationDate.HasValue && TerminationDate.Value.Date < HireDate.Date)
        {
            yield return new ValidationResult(
                "Termination date cannot be before hire date.",
                new[] { nameof(TerminationDate), nameof(HireDate) });
        }
    }
}
