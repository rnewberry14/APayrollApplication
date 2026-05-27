using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public class W2ImportError
{
    public int W2ImportErrorId { get; set; }

    public int W2ImportBatchId { get; set; }

    public W2ImportBatch? W2ImportBatch { get; set; }

    public int? W2ImportRecordId { get; set; }

    public W2ImportRecord? W2ImportRecord { get; set; }

    public W2ImportErrorSeverity Severity { get; set; } = W2ImportErrorSeverity.Warning;

    [StringLength(100)]
    public string FieldName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum W2ImportErrorSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2
}
