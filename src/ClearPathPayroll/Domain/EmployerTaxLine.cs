using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents an employer tax line item for a payroll run.
/// </summary>
[Table("EmployerTaxLines")]
public class EmployerTaxLine
{
    /// <summary>
    /// Unique identifier for the employer tax line.
    /// </summary>
    [Key]
    public int EmployerTaxLineId { get; set; }

    /// <summary>
    /// Foreign key to the payroll run.
    /// </summary>
    [Required]
    public int PayrollRunId { get; set; }

    /// <summary>
    /// Navigation property to the payroll run.
    /// </summary>
    [ForeignKey("PayrollRunId")]
    public PayrollRun? PayrollRun { get; set; }

    /// <summary>
    /// Foreign key to the employee.
    /// </summary>
    public int? EmployeeId { get; set; }

    /// <summary>
    /// Navigation property to the employee.
    /// </summary>
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }

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