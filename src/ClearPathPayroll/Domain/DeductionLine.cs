using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents a deduction line item for an employee in a payroll run.
/// </summary>
[Table("DeductionLines")]
public class DeductionLine
{
    /// <summary>
    /// Unique identifier for the deduction line.
    /// </summary>
    [Key]
    public int DeductionLineId { get; set; }

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
    /// Description of the deduction.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Amount deducted.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
}