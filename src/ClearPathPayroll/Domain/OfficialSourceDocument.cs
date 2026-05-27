using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

[Table("OfficialSourceDocuments")]
public class OfficialSourceDocument
{
    [Key]
    public int OfficialSourceDocumentId { get; set; }

    [Required]
    [StringLength(200)]
    public string SourceName { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string PublicationTitle { get; set; } = string.Empty;

    public DateTime? RevisionDate { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public DateTime? RetrievedDate { get; set; }

    [Required]
    [StringLength(1000)]
    public string OfficialSourceUrl { get; set; } = string.Empty;

    [Required]
    public string ExcerptText { get; set; } = string.Empty;

    public bool IsCurrent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OfficialSourceReviewLog> ReviewLogs { get; set; } = new();
}
