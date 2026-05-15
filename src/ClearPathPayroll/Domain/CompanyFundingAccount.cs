using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Company's bank account used for funding direct deposit batches.
/// Stores tokenized account information for security.
/// </summary>
[Table("CompanyFundingAccounts")]
public class CompanyFundingAccount
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int CompanyFundingAccountId { get; set; }

    /// <summary>
    /// Foreign key to Company.
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Bank name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string BankName { get; set; } = string.Empty;

    /// <summary>
    /// Account type (typically Checking).
    /// </summary>
    [Required]
    public BankAccountType AccountType { get; set; } = BankAccountType.Checking;

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
    /// Account nickname/description (e.g., "Primary Operating Account").
    /// </summary>
    [StringLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Whether this is the primary funding account for the company.
    /// </summary>
    public bool IsPrimary { get; set; } = true;

    /// <summary>
    /// Whether this account is active for use.
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
    /// Navigation property to Company.
    /// </summary>
    public virtual Company? Company { get; set; }

    /// <summary>
    /// Navigation to DirectDepositBatches that use this account.
    /// </summary>
    public virtual ICollection<DirectDepositBatch> DirectDepositBatches { get; set; } = new List<DirectDepositBatch>();
}
