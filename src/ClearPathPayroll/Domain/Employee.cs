using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Employment status enumeration.
/// </summary>
public enum EmploymentStatus
{
    Active,
    Inactive,
    Terminated
}

/// <summary>
/// Pay type enumeration.
/// </summary>
public enum PayType
{
    Hourly,
    Salary
}

/// <summary>
/// Represents an employee in the payroll system.
/// </summary>
[Table("Employees")]
public class Employee
{
    /// <summary>
    /// Unique identifier for the employee.
    /// </summary>
    [Key]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Foreign key to the company.
    /// </summary>
    [Required]
    public int CompanyId { get; set; }

    /// <summary>
    /// Navigation property to the company.
    /// </summary>
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    /// <summary>
    /// First name of the employee.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the employee.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Middle initial of the employee.
    /// </summary>
    [StringLength(1)]
    public string? MiddleInitial { get; set; }

    /// <summary>
    /// Last 4 digits of the SSN.
    /// </summary>
    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string SSNLast4 { get; set; } = string.Empty;

    /// <summary>
    /// Placeholder for encrypted full SSN. Do not store plain-text SSNs.
    /// </summary>
    [StringLength(256)] // Placeholder length
    public string? FullSSNEncryptedPlaceholder { get; set; }

    /// <summary>
    /// Date of birth of the employee.
    /// </summary>
    [Required]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Hire date of the employee.
    /// </summary>
    [Required]
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Termination date of the employee (nullable).
    /// </summary>
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    /// Employment status.
    /// </summary>
    [Required]
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;

    /// <summary>
    /// Pay type: Hourly or Salary.
    /// </summary>
    [Required]
    public PayType PayType { get; set; }

    /// <summary>
    /// Hourly rate (for hourly employees).
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? HourlyRate { get; set; }

    /// <summary>
    /// Annual salary (for salaried employees).
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? AnnualSalary { get; set; }

    /// <summary>
    /// Primary work location ID (placeholder for future use).
    /// </summary>
    public int? PrimaryWorkLocationId { get; set; }

    /// <summary>
    /// Residence address of the employee.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ResidenceAddress { get; set; } = string.Empty;

    /// <summary>
    /// City of residence.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State of residence.
    /// </summary>
    [Required]
    [StringLength(2)]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Zip code of residence.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the employee.
    /// </summary>
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number of the employee.
    /// </summary>
    [Phone]
    [StringLength(15)]
    public string? Phone { get; set; }

    /// <summary>
    /// Date and time when the employee was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the employee was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}