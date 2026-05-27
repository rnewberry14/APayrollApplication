using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

public enum BankAccountType
{
    Checking,
    Savings,
    PayCardPlaceholder
}

public enum VerificationStatus
{
    Pending,
    Verified,
    Failed,
    Inactive
}

public enum DirectDepositDepositType
{
    FixedAmount,
    Percentage,
    Remainder
}

public enum PrenoteStatus
{
    NotStarted,
    PendingPlaceholder,
    CompletePlaceholder,
    FailedPlaceholder
}

[Table("EmployeeBankAccounts")]
public class EmployeeBankAccount : IValidatableObject
{
    public int EmployeeBankAccountId { get; set; }

    public int EmployeeId { get; set; }

    [Range(1, 5)]
    public int PriorityOrder { get; set; } = 1;

    [Required]
    [StringLength(100)]
    public string BankName { get; set; } = string.Empty;

    [Required]
    public BankAccountType AccountType { get; set; }

    [Required]
    [StringLength(256)]
    public string RoutingNumberToken { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string AccountNumberToken { get; set; } = string.Empty;

    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string Last4 { get; set; } = string.Empty;

    [Required]
    public DirectDepositDepositType DepositType { get; set; } = DirectDepositDepositType.Remainder;

    [Range(0, double.MaxValue)]
    public decimal? DepositAmount { get; set; }

    [Range(0, 100)]
    public decimal? DepositPercent { get; set; }

    public bool IsRemainderAccount { get; set; } = true;

    [Required]
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

    public PrenoteStatus PrenoteStatus { get; set; } = PrenoteStatus.NotStarted;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? CreatedByUserId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<DirectDepositItem> DirectDepositItems { get; set; } = new List<DirectDepositItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RoutingNumberToken.Length == 9 && RoutingNumberToken.All(char.IsDigit))
        {
            yield return new ValidationResult(
                "RoutingNumberToken must contain a token placeholder, not a raw routing number.",
                new[] { nameof(RoutingNumberToken) });
        }

        if (AccountNumberToken.Length >= 5 && AccountNumberToken.All(char.IsDigit))
        {
            yield return new ValidationResult(
                "AccountNumberToken must contain a token placeholder, not a raw account number.",
                new[] { nameof(AccountNumberToken) });
        }

        if (DepositType == DirectDepositDepositType.FixedAmount && (!DepositAmount.HasValue || DepositAmount.Value <= 0))
        {
            yield return new ValidationResult(
                "Fixed amount direct deposit requires a deposit amount greater than zero.",
                new[] { nameof(DepositAmount) });
        }

        if (DepositType == DirectDepositDepositType.Percentage && (!DepositPercent.HasValue || DepositPercent.Value <= 0 || DepositPercent.Value > 100))
        {
            yield return new ValidationResult(
                "Percentage direct deposit requires a deposit percent greater than zero and no more than 100.",
                new[] { nameof(DepositPercent) });
        }

        if (DepositType == DirectDepositDepositType.Remainder && !IsRemainderAccount)
        {
            yield return new ValidationResult(
                "Remainder direct deposit must be marked as the remainder account.",
                new[] { nameof(IsRemainderAccount) });
        }
    }
}
