using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public class ImportError
{
    public int ImportErrorId { get; set; }

    public int ImportBatchId { get; set; }

    public ImportBatch? ImportBatch { get; set; }

    public int? ImportRowId { get; set; }

    public ImportRow? ImportRow { get; set; }

    public int? RowNumber { get; set; }

    [StringLength(100)]
    public string? ColumnName { get; set; }

    [Required]
    [StringLength(100)]
    public string ErrorCode { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
