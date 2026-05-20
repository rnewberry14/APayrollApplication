namespace ClearPathPayroll.Integrations;

/// <summary>
/// ACH payment status enumeration.
/// </summary>
public enum AchPaymentStatus
{
    /// <summary>
    /// Payment is pending processing.
    /// </summary>
    Pending,

    /// <summary>
    /// Payment has been transmitted to ACH processor.
    /// </summary>
    Transmitted,

    /// <summary>
    /// Payment was successfully settled and credited to employee.
    /// </summary>
    Settled,

    /// <summary>
    /// Payment was returned by receiving bank (e.g., invalid account).
    /// </summary>
    Returned,

    /// <summary>
    /// Payment was rejected by ACH processor (format/validation error).
    /// </summary>
    Rejected,

    /// <summary>
    /// Payment is under dispute.
    /// </summary>
    Disputed,

    /// <summary>
    /// Payment processing failed.
    /// </summary>
    Failed
}

/// <summary>
/// Represents a single payment item in an ACH batch.
/// </summary>
public class AchPaymentItem
{
    /// <summary>
    /// Employee ID (for reference).
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Net pay amount to be deposited.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Tokenized routing number (placeholder, not raw number).
    /// </summary>
    public string RoutingNumberToken { get; set; } = string.Empty;

    /// <summary>
    /// Tokenized account number (placeholder, not raw number).
    /// </summary>
    public string AccountNumberToken { get; set; } = string.Empty;

    /// <summary>
    /// Last 4 digits of account (safe for display).
    /// </summary>
    public string Last4 { get; set; } = string.Empty;

    /// <summary>
    /// Account type (Checking or Savings).
    /// </summary>
    public string AccountType { get; set; } = "Checking";

    /// <summary>
    /// Individual name for ACH transaction.
    /// </summary>
    public string IndividualName { get; set; } = string.Empty;

    /// <summary>
    /// Status of this payment item.
    /// </summary>
    public AchPaymentStatus Status { get; set; } = AchPaymentStatus.Pending;

    /// <summary>
    /// External reference from ACH processor (for tracking).
    /// </summary>
    public string? ExternalReference { get; set; }

    /// <summary>
    /// Return code if payment was returned (e.g., "R01" = Invalid Account Number).
    /// </summary>
    public string? ReturnCode { get; set; }

    /// <summary>
    /// Return reason description.
    /// </summary>
    public string? ReturnDescription { get; set; }

    /// <summary>
    /// Error message if payment failed to process.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ACH batch submission request.
/// </summary>
public class AchBatchRequest
{
    /// <summary>
    /// Batch ID from the payroll system (for reference).
    /// </summary>
    public int BatchId { get; set; }

    /// <summary>
    /// Company ID (for reference and routing).
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Company name for ACH file header.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Company EIN for ACH file header.
    /// </summary>
    public string CompanyEin { get; set; } = string.Empty;

    /// <summary>
    /// Funding account routing number (tokenized).
    /// </summary>
    public string FundingRoutingNumberToken { get; set; } = string.Empty;

    /// <summary>
    /// Funding account number (tokenized).
    /// </summary>
    public string FundingAccountNumberToken { get; set; } = string.Empty;

    /// <summary>
    /// Effective date for ACH processing.
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Settlement date (typically effective date + 1).
    /// </summary>
    public DateTime SettlementDate { get; set; }

    /// <summary>
    /// Payment items in this batch.
    /// </summary>
    public List<AchPaymentItem> Items { get; set; } = new();

    /// <summary>
    /// Whether to process in sandbox/test mode (no real transfers).
    /// </summary>
    public bool SandboxMode { get; set; } = true;

    /// <summary>
    /// Optional external reference for tracking.
    /// </summary>
    public string? ExternalReference { get; set; }

    /// <summary>
    /// API version for versioning.
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// Total amount of batch (sum of item amounts).
    /// </summary>
    public decimal TotalAmount => Items.Sum(i => i.Amount);

    /// <summary>
    /// Item count in batch.
    /// </summary>
    public int ItemCount => Items.Count;
}

/// <summary>
/// Individual payment item response.
/// </summary>
public class AchPaymentItemResponse
{
    /// <summary>
    /// Employee ID.
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Payment amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Status of this payment.
    /// </summary>
    public AchPaymentStatus Status { get; set; }

    /// <summary>
    /// External trace number for this payment (for reconciliation).
    /// </summary>
    public string? TraceNumber { get; set; }

    /// <summary>
    /// Return code if applicable.
    /// </summary>
    public string? ReturnCode { get; set; }

    /// <summary>
    /// Error/return description.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// ACH batch submission response.
/// </summary>
public class AchBatchResponse
{
    /// <summary>
    /// Whether the batch was successfully submitted.
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Response timestamp.
    /// </summary>
    public DateTime ResponseTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// External batch reference from ACH processor (for tracking).
    /// </summary>
    public string? BatchReference { get; set; }

    /// <summary>
    /// Submission timestamp.
    /// </summary>
    public DateTime? SubmissionTimestamp { get; set; }

    /// <summary>
    /// Settlement date provided by processor.
    /// </summary>
    public DateTime? SettlementDate { get; set; }

    /// <summary>
    /// Individual item responses.
    /// </summary>
    public List<AchPaymentItemResponse> ItemResponses { get; set; } = new();

    /// <summary>
    /// Error message if batch submission failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether batch was processed in sandbox/test mode.
    /// </summary>
    public bool SandboxMode { get; set; } = true;

    /// <summary>
    /// API version used.
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// Total amount submitted.
    /// </summary>
    public decimal TotalAmount => ItemResponses.Sum(i => i.Amount);

    /// <summary>
    /// Count of successfully processed items.
    /// </summary>
    public int SuccessfulItemCount => ItemResponses.Count(i => i.Status == AchPaymentStatus.Settled);

    /// <summary>
    /// Count of rejected or returned items.
    /// </summary>
    public int FailedItemCount => ItemResponses.Count(i => i.Status == AchPaymentStatus.Returned || i.Status == AchPaymentStatus.Rejected);
}
