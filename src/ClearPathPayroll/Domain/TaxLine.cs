using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents a tax line item for an employee in a payroll run.
/// </summary>
[Table("TaxLines")]
public class TaxLine
{
    /// <summary>
    /// Unique identifier for the tax line.
    /// </summary>
    [Key]
    public int TaxLineId { get; set; }

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
    /// Type of tax.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string TaxType { get; set; } = string.Empty;

    /// <summary>
    /// Amount of tax.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
}