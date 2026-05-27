using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public class W2ImportBatch
{
    public int W2ImportBatchId { get; set; }

    public int? CompanyId { get; set; }

    public Company? Company { get; set; }

    [Required]
    [StringLength(260)]
    public string FileName { get; set; } = string.Empty;

    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string ImportedByUserId { get; set; } = string.Empty;

    public W2ImportStatus ImportStatus { get; set; } = W2ImportStatus.Uploaded;

    public int RecordCount { get; set; }

    public int ErrorCount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<W2ImportRecord> Records { get; set; } = new();

    public List<W2ImportError> Errors { get; set; } = new();
}

public enum W2ImportStatus
{
    Uploaded = 0,
    Parsed = 1,
    NeedsReview = 2,
    Confirmed = 3,
    Failed = 4,
    Cancelled = 5
}
