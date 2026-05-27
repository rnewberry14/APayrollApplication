using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public class ImportMapping
{
    public int ImportMappingId { get; set; }

    public int ImportBatchId { get; set; }

    public ImportBatch? ImportBatch { get; set; }

    [Required]
    [StringLength(100)]
    public string SourceColumn { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string TargetField { get; set; } = string.Empty;

    public bool IsRequired { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
