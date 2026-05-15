using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Direct deposit item status.
/// </summary>
public enum DirectDepositItemStatus
{
    /// <summary>
    /// Item is pending processing.
    /// </summary>
    Pending,

    /// <summary>
    /// Item has been transmitted to ACH processor.
    /// </summary>
    Transmitted,

    /// <summary>
    /// Item was successfully processed and settled.
    /// </summary>
    Settled,

    /// <summary>
    /// Item was rejected by ACH processor.
    /// </summary>
    Rejected,

    /// <summary>
    /// Item was returned by the receiving bank.
    /// </summary>
    Returned,

    /// <summary>
    /// Item was reversed after settlement.
    /// </summary>
    Reversed,

    /// <summary>
    /// Item processing failed.
    /// </summary>
    Failed
}

/// <summary>
/// Individual direct deposit item within a batch.
/// Represents payment to a single employee.
/// </summary>
[Table("DirectDepositItems")]
public class DirectDepositItem
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int BatchItemId { get; set; }

    /// <summary>
    /// Foreign key to DirectDepositBatch.
    /// </summary>
    public int BatchId { get; set; }

    /// <summary>
    /// Foreign key to Employee.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Foreign key to EmployeeBankAccount.
    /// </summary>
    public int EmployeeBankAccountId { get; set; }

    /// <summary>
    /// Net pay amount to be deposited.
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Current status of the item.
    /// </summary>
    [Required]
    public DirectDepositItemStatus Status { get; set; } = DirectDepositItemStatus.Pending;

    /// <summary>
    /// External reference from ACH processor (for tracking and reconciliation).
    /// </summary>
    [StringLength(256)]
    public string? ExternalItemReference { get; set; }

    /// <summary>
    /// Return code if item was returned/rejected (e.g., "R01", "R02", etc.).
    /// </summary>
    [StringLength(10)]
    public string? ReturnCode { get; set; }

    /// <summary>
    /// Return/rejection description (e.g., "Invalid account number").
    /// </summary>
    [StringLength(500)]
    public string? ReturnDescription { get; set; }

    /// <summary>
    /// Timestamp when item was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when item was transmitted.
    /// </summary>
    public DateTime? TransmittedAt { get; set; }

    /// <summary>
    /// Timestamp when item was settled.
    /// </summary>
    public DateTime? SettledAt { get; set; }

    /// <summary>
    /// Navigation property to DirectDepositBatch.
    /// </summary>
    public virtual DirectDepositBatch? Batch { get; set; }

    /// <summary>
    /// Navigation property to Employee.
    /// </summary>
    public virtual Employee? Employee { get; set; }

    /// <summary>
    /// Navigation property to EmployeeBankAccount.
    /// </summary>
    public virtual EmployeeBankAccount? BankAccount { get; set; }
}
