using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents an earning line item for an employee in a payroll run.
/// </summary>
[Table("EarningLines")]
public class EarningLine
{
    /// <summary>
    /// Unique identifier for the earning line.
    /// </summary>
    [Key]
    public int EarningLineId { get; set; }

    /// <summary>
    /// Foreign key to the payroll run employee.
    /// </summary>
    [Required]
    public int PayrollRunEmployeeId { get; set; }

    /// <summary>
    /// Navigation property to the payroll run employee.
    /// </summary>
    [ForeignKey("PayrollRunEmployeeId")]
    public PayrollRunEmployee? PayrollRunEmployee { get; set; }

    /// <summary>
    /// Description of the earning.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Hours worked (for hourly employees).
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? Hours { get; set; }

    /// <summary>
    /// Rate per hour.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? Rate { get; set; }

    /// <summary>
    /// Amount earned.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
}