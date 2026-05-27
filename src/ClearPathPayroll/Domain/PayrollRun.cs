using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Payroll run status enumeration.
/// </summary>
public enum PayrollStatus
{
    Draft,
    Calculated,
    Approved,
    Submitted,
    Completed,
    Voided
}

/// <summary>
/// Represents a payroll run.
/// </summary>
[Table("PayrollRuns")]
public class PayrollRun
{
    /// <summary>
    /// Unique identifier for the payroll run.
    /// </summary>
    [Key]
    public int PayrollRunId { get; set; }

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
    /// Foreign key to the pay schedule.
    /// </summary>
    [Required]
    public int PayScheduleId { get; set; }

    /// <summary>
    /// Navigation property to the pay schedule.
    /// </summary>
    [ForeignKey("PayScheduleId")]
    public PaySchedule? PaySchedule { get; set; }

    /// <summary>
    /// Start date of the pay period.
    /// </summary>
    [Required]
    public DateTime PayPeriodStart { get; set; }

    /// <summary>
    /// End date of the pay period.
    /// </summary>
    [Required]
    public DateTime PayPeriodEnd { get; set; }

    /// <summary>
    /// Pay date.
    /// </summary>
    [Required]
    public DateTime PayDate { get; set; }

    /// <summary>
    /// Status of the payroll run.
    /// </summary>
    [Required]
    public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

    [StringLength(50)]
    public string PayrollMode { get; set; } = "Regular payroll";

    /// <summary>
    /// Total gross pay.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalGrossPay { get; set; }

    /// <summary>
    /// Total employee taxes.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalEmployeeTaxes { get; set; }

    /// <summary>
    /// Total employer taxes.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalEmployerTaxes { get; set; }

    /// <summary>
    /// Total deductions.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalDeductions { get; set; }

    /// <summary>
    /// Total net pay.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalNetPay { get; set; }

    /// <summary>
    /// User ID who created the payroll run.
    /// </summary>
    [StringLength(100)]
    public string? CreatedByUserId { get; set; }

    /// <summary>
    /// User ID who approved the payroll run.
    /// </summary>
    [StringLength(100)]
    public string? ApprovedByUserId { get; set; }

    /// <summary>
    /// Date and time when the payroll run was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the payroll run was approved.
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Navigation property to payroll run employees.
    /// </summary>
    public ICollection<PayrollRunEmployee> PayrollRunEmployees { get; set; } = new List<PayrollRunEmployee>();

    /// <summary>
    /// Navigation property to employer tax lines.
    /// </summary>
    public ICollection<EmployerTaxLine> EmployerTaxLines { get; set; } = new List<EmployerTaxLine>();
}
