using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents an employee in a payroll run.
/// </summary>
[Table("PayrollRunEmployees")]
public class PayrollRunEmployee
{
    /// <summary>
    /// Unique identifier for the payroll run employee.
    /// </summary>
    [Key]
    public int PayrollRunEmployeeId { get; set; }

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
    [Required]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Navigation property to the employee.
    /// </summary>
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }

    /// <summary>
    /// Gross pay for this employee.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal GrossPay { get; set; }

    /// <summary>
    /// Total deductions for this employee.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalDeductions { get; set; }

    /// <summary>
    /// Total taxes for this employee.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalTaxes { get; set; }

    /// <summary>
    /// Net pay for this employee.
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal NetPay { get; set; }

    /// <summary>
    /// Navigation property to earning lines.
    /// </summary>
    public ICollection<EarningLine> EarningLines { get; set; } = new List<EarningLine>();

    /// <summary>
    /// Navigation property to deduction lines.
    /// </summary>
    public ICollection<DeductionLine> DeductionLines { get; set; } = new List<DeductionLine>();

    /// <summary>
    /// Navigation property to tax lines.
    /// </summary>
    public ICollection<TaxLine> TaxLines { get; set; } = new List<TaxLine>();

    /// <summary>
    /// Navigation property to net pay lines.
    /// </summary>
    public ICollection<NetPayLine> NetPayLines { get; set; } = new List<NetPayLine>();
}