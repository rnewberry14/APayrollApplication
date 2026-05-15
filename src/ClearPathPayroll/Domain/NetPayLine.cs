using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents a net pay line item for an employee in a payroll run.
/// </summary>
[Table("NetPayLines")]
public class NetPayLine
{
    /// <summary>
    /// Unique identifier for the net pay line.
    /// </summary>
    [Key]
    public int NetPayLineId { get; set; }

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
    /// Amount of net pay.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment method.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;
}