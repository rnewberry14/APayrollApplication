using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Direct deposit batch processing status.
/// </summary>
public enum DirectDepositBatchStatus
{
    /// <summary>
    /// Batch is being prepared, items can still be added/modified.
    /// </summary>
    Draft,

    /// <summary>
    /// Batch is complete and ready to submit to ACH processor.
    /// </summary>
    Ready,

    /// <summary>
    /// Batch has been submitted to ACH processor.
    /// </summary>
    Submitted,

    /// <summary>
    /// Batch settlement complete; funds transferred.
    /// </summary>
    Settled,

    /// <summary>
    /// Batch submission failed (usually due to validation or processor error).
    /// </summary>
    Failed,

    /// <summary>
    /// Batch was reversed after settlement (refunds issued).
    /// </summary>
    Reversed
}

/// <summary>
/// Direct deposit batch containing multiple employee payments.
/// Represents a single ACH file submission to the ACH processor.
/// </summary>
[Table("DirectDepositBatches")]
public class DirectDepositBatch
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int BatchId { get; set; }

    /// <summary>
    /// Foreign key to PayrollRun.
    /// </summary>
    public int PayrollRunId { get; set; }

    /// <summary>
    /// Foreign key to Company.
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Foreign key to CompanyFundingAccount used for this batch.
    /// </summary>
    public int CompanyFundingAccountId { get; set; }

    /// <summary>
    /// Date on which funds will be deposited to employee accounts.
    /// </summary>
    [Required]
    public DateTime PayDate { get; set; }

    /// <summary>
    /// Current status of the batch.
    /// </summary>
    [Required]
    public DirectDepositBatchStatus Status { get; set; } = DirectDepositBatchStatus.Draft;

    /// <summary>
    /// Total amount to be transferred via this batch.
    /// </summary>
    [Required]
    public decimal TotalAmount { get; set; } = 0m;

    /// <summary>
    /// Batch description/reference (e.g., "Payroll 05-15-2026").
    /// </summary>
    [StringLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when batch was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when batch was submitted to ACH processor.
    /// </summary>
    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// Timestamp when batch was settled.
    /// </summary>
    public DateTime? SettledAt { get; set; }

    /// <summary>
    /// External reference from ACH processor (for tracking).
    /// </summary>
    [StringLength(256)]
    public string? ExternalBatchReference { get; set; }

    /// <summary>
    /// Optional error message if batch failed.
    /// </summary>
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// User who created the batch (for audit).
    /// </summary>
    [StringLength(100)]
    public string? CreatedByUserId { get; set; }

    /// <summary>
    /// User who submitted the batch (for audit).
    /// </summary>
    [StringLength(100)]
    public string? SubmittedByUserId { get; set; }

    /// <summary>
    /// Navigation property to PayrollRun.
    /// </summary>
    public virtual PayrollRun? PayrollRun { get; set; }

    /// <summary>
    /// Navigation property to Company.
    /// </summary>
    public virtual Company? Company { get; set; }

    /// <summary>
    /// Navigation property to CompanyFundingAccount.
    /// </summary>
    public virtual CompanyFundingAccount? FundingAccount { get; set; }

    /// <summary>
    /// Navigation to DirectDepositItems in this batch.
    /// </summary>
    public virtual ICollection<DirectDepositItem> Items { get; set; } = new List<DirectDepositItem>();

    /// <summary>
    /// Get total number of items in batch.
    /// </summary>
    public int ItemCount => Items.Count;

    /// <summary>
    /// Whether the batch is locked from editing (submitted or beyond).
    /// </summary>
    public bool IsLocked => Status >= DirectDepositBatchStatus.Ready;
}
