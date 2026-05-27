using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

public sealed class OfficialSourceDisplayModel
{
    public OfficialSourceDocument Document { get; set; } = new();
    public DateTime? LastReviewedAt { get; set; }
    public bool NeedsCurrentnessWarning { get; set; }
    public bool NeedsAgeReviewWarning { get; set; }
}

public class OfficialSourceService
{
    public const string NotCurrentWarning = "This source has not been marked current. Verify the official source before relying on it.";
    public const string AgeReviewWarning = "This source may need review. Verify the current official source.";
    public const string ReviewNote = "Review recorded. Currentness was not automatically certified.";

    private static readonly TimeSpan ReviewAgeThreshold = TimeSpan.FromDays(365);
    private readonly PayrollDbContext _context;

    public OfficialSourceService(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task<List<OfficialSourceDisplayModel>> GetOfficialSourcesAsync(DateTime? asOfDate = null)
    {
        var currentDate = (asOfDate ?? DateTime.UtcNow).Date;
        var documents = await _context.OfficialSourceDocuments
            .Include(d => d.ReviewLogs)
            .OrderBy(d => d.SourceName)
            .ThenBy(d => d.PublicationTitle)
            .ToListAsync();

        return documents.Select(document => BuildDisplayModel(document, currentDate)).ToList();
    }

    public async Task<OfficialSourceReviewLog> MarkReviewedAsync(int officialSourceDocumentId, string reviewedByUserId)
    {
        var document = await _context.OfficialSourceDocuments
            .FirstOrDefaultAsync(d => d.OfficialSourceDocumentId == officialSourceDocumentId);

        if (document == null)
        {
            throw new InvalidOperationException($"Official source document {officialSourceDocumentId} was not found.");
        }

        var reviewLog = new OfficialSourceReviewLog
        {
            OfficialSourceDocumentId = officialSourceDocumentId,
            ReviewedAt = DateTime.UtcNow,
            ReviewedByUserId = string.IsNullOrWhiteSpace(reviewedByUserId) ? "local-prototype-user" : reviewedByUserId,
            Note = ReviewNote
        };

        _context.OfficialSourceReviewLogs.Add(reviewLog);
        await _context.SaveChangesAsync();
        return reviewLog;
    }

    public static OfficialSourceDisplayModel BuildDisplayModel(OfficialSourceDocument document, DateTime currentDate)
    {
        return new OfficialSourceDisplayModel
        {
            Document = document,
            LastReviewedAt = document.ReviewLogs
                .OrderByDescending(log => log.ReviewedAt)
                .Select(log => (DateTime?)log.ReviewedAt)
                .FirstOrDefault(),
            NeedsCurrentnessWarning = !document.IsCurrent,
            NeedsAgeReviewWarning = IsOlderThanThreshold(document.RetrievedDate, currentDate) ||
                                    IsOlderThanThreshold(document.RevisionDate, currentDate)
        };
    }

    private static bool IsOlderThanThreshold(DateTime? sourceDate, DateTime currentDate)
    {
        return sourceDate.HasValue && currentDate.Date - sourceDate.Value.Date > ReviewAgeThreshold;
    }
}
