using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Deduction type enumeration.
/// </summary>
public enum DeductionType
{
    PreTaxFixedAmount,
    PreTaxPercentage,
    PostTaxFixedAmount,
    PostTaxPercentage
}

/// <summary>
/// Input for a deduction to be calculated.
/// </summary>
public class DeductionInput
{
    /// <summary>
    /// Description of the deduction.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of deduction.
    /// </summary>
    [Required]
    public DeductionType Type { get; set; }

    /// <summary>
    /// Amount (for fixed deductions) or percentage (for percentage deductions).
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
}

/// <summary>
/// Result line for a calculated deduction.
/// </summary>
public class DeductionResultLine
{
    /// <summary>
    /// Description of the deduction.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of deduction.
    /// </summary>
    public DeductionType Type { get; set; }

    /// <summary>
    /// Amount deducted.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Reason if deduction was adjusted or skipped.
    /// </summary>
    public string? AdjustmentReason { get; set; }
}

/// <summary>
/// Result of deduction calculation for an employee.
/// </summary>
public class DeductionCalculationResult
{
    /// <summary>
    /// Total pre-tax deductions.
    /// </summary>
    public decimal TotalPreTaxDeductions { get; set; }

    /// <summary>
    /// Total post-tax deductions.
    /// </summary>
    public decimal TotalPostTaxDeductions { get; set; }

    /// <summary>
    /// Total deductions (pre-tax + post-tax).
    /// </summary>
    public decimal TotalDeductions { get; set; }

    /// <summary>
    /// Detailed deduction line items.
    /// </summary>
    public List<DeductionResultLine> DeductionLines { get; set; } = new();

    /// <summary>
    /// Indicates if any deductions were reduced due to insufficient wages.
    /// </summary>
    public bool HasAdjustments { get; set; }
}