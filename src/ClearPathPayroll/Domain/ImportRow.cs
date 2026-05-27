using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public class ImportRow
{
    public int ImportRowId { get; set; }

    public int ImportBatchId { get; set; }

    public ImportBatch? ImportBatch { get; set; }

    public int RowNumber { get; set; }

    public ImportRowStatus Status { get; set; } = ImportRowStatus.PendingReview;

    [Required]
    public string RowDataJson { get; set; } = "{}";

    [StringLength(500)]
    public string? ErrorSummary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ImportedAt { get; set; }
}

public enum ImportRowStatus
{
    PendingReview = 0,
    Valid = 1,
    Error = 2,
    Imported = 3,
    Skipped = 4
}
