using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

[Table("OfficialSourceReviewLogs")]
public class OfficialSourceReviewLog
{
    [Key]
    public int OfficialSourceReviewLogId { get; set; }

    public int OfficialSourceDocumentId { get; set; }

    public OfficialSourceDocument? OfficialSourceDocument { get; set; }

    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string ReviewedByUserId { get; set; } = string.Empty;

    [StringLength(500)]
    public string Note { get; set; } = "Review recorded. Currentness was not automatically certified.";
}
