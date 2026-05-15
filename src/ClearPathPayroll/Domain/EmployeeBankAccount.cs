using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Bank account type enumeration.
/// </summary>
public enum BankAccountType
{
    Checking,
    Savings
}

/// <summary>
/// Verification status for employee bank accounts.
/// </summary>
public enum VerificationStatus
{
    Pending,
    Verified,
    Failed,
    Inactive
}

/// <summary>
/// Employee's bank account for direct deposit.
/// Stores tokenized account information for security.
/// </summary>
[Table("EmployeeBankAccounts")]
public class EmployeeBankAccount
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int EmployeeBankAccountId { get; set; }

    /// <summary>
    /// Foreign key to Employee.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Bank name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// Account type (Checking or Savings).
    /// </summary>
    [Required]
    public BankAccountType AccountType { get; set; }

    /// <summary>
    /// Tokenized routing number (placeholder for token, not raw number).
    /// </summary>
    [Required]
    [StringLength(256)]
    public string RoutingNumberToken { get; set; } = string.Empty;

    /// <summary>
    /// Tokenized account number (placeholder for token, not raw number).
    /// </summary>
    [Required]
    [StringLength(256)]
    public string AccountNumberToken { get; set; } = string.Empty;

    /// <summary>
    /// Last 4 digits of account number (safe for display).
    /// </summary>
    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string Last4 { get; set; } = string.Empty;

    /// <summary>
    /// Verification status of the account.
    /// </summary>
    [Required]
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

    /// <summary>
    /// Whether this account is active for direct deposits.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when account was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional user who created the record (for audit).
    /// </summary>
    [StringLength(100)]
    public string? CreatedByUserId { get; set; }

    /// <summary>
    /// Navigation property to Employee.
    /// </summary>
    public virtual Employee? Employee { get; set; }

    /// <summary>
    /// Navigation to DirectDepositItems that use this account.
    /// </summary>
    public virtual ICollection<DirectDepositItem> DirectDepositItems { get; set; } = new List<DirectDepositItem>();
}
