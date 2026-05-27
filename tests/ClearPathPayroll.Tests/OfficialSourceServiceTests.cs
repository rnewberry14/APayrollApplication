using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class OfficialSourceServiceTests
{
    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    [Fact]
    public void BuildDisplayModel_ShowsWarning_WhenDocumentIsNotCurrent()
    {
        var document = new OfficialSourceDocument
        {
            SourceName = "Official Agency",
            PublicationTitle = "Publication",
            IsCurrent = false,
            RetrievedDate = new DateTime(2026, 1, 1)
        };

        var model = OfficialSourceService.BuildDisplayModel(document, new DateTime(2026, 5, 26));

        Assert.True(model.NeedsCurrentnessWarning);
        Assert.False(model.NeedsAgeReviewWarning);
    }

    [Fact]
    public void BuildDisplayModel_ShowsReviewWarning_WhenRetrievedDateIsOlderThanTwelveMonths()
    {
        var document = new OfficialSourceDocument
        {
            SourceName = "Official Agency",
            PublicationTitle = "Publication",
            IsCurrent = true,
            RetrievedDate = new DateTime(2025, 5, 25)
        };

        var model = OfficialSourceService.BuildDisplayModel(document, new DateTime(2026, 5, 26));

        Assert.False(model.NeedsCurrentnessWarning);
        Assert.True(model.NeedsAgeReviewWarning);
    }

    [Fact]
    public void BuildDisplayModel_ShowsReviewWarning_WhenRevisionDateIsOlderThanTwelveMonths()
    {
        var document = new OfficialSourceDocument
        {
            SourceName = "Official Agency",
            PublicationTitle = "Publication",
            IsCurrent = true,
            RevisionDate = new DateTime(2025, 5, 25)
        };

        var model = OfficialSourceService.BuildDisplayModel(document, new DateTime(2026, 5, 26));

        Assert.True(model.NeedsAgeReviewWarning);
    }

    [Fact]
    public async Task MarkReviewedAsync_CreatesReviewLog_AndDoesNotMarkCurrent()
    {
        await using var context = CreateContext("official_source_review");
        var document = new OfficialSourceDocument
        {
            SourceName = "Official Agency",
            PublicationTitle = "Publication",
            OfficialSourceUrl = "https://agency.example/official",
            ExcerptText = "Official excerpt text.",
            IsCurrent = false,
            RetrievedDate = new DateTime(2025, 1, 1)
        };
        context.OfficialSourceDocuments.Add(document);
        await context.SaveChangesAsync();

        var service = new OfficialSourceService(context);

        var log = await service.MarkReviewedAsync(document.OfficialSourceDocumentId, "reviewer");

        Assert.True(log.OfficialSourceReviewLogId > 0);
        Assert.Equal("reviewer", log.ReviewedByUserId);
        Assert.Equal(OfficialSourceService.ReviewNote, log.Note);

        var reloadedDocument = await context.OfficialSourceDocuments
            .Include(d => d.ReviewLogs)
            .SingleAsync(d => d.OfficialSourceDocumentId == document.OfficialSourceDocumentId);
        Assert.False(reloadedDocument.IsCurrent);
        Assert.Single(reloadedDocument.ReviewLogs);
    }

    [Fact]
    public async Task GetOfficialSourcesAsync_ReturnsLastReviewedDate()
    {
        await using var context = CreateContext("official_source_last_reviewed");
        var document = new OfficialSourceDocument
        {
            SourceName = "Official Agency",
            PublicationTitle = "Publication",
            OfficialSourceUrl = "https://agency.example/official",
            ExcerptText = "Official excerpt text.",
            IsCurrent = true,
            RetrievedDate = new DateTime(2026, 1, 1)
        };
        document.ReviewLogs.Add(new OfficialSourceReviewLog { ReviewedAt = new DateTime(2026, 1, 1), ReviewedByUserId = "one" });
        document.ReviewLogs.Add(new OfficialSourceReviewLog { ReviewedAt = new DateTime(2026, 2, 1), ReviewedByUserId = "two" });
        context.OfficialSourceDocuments.Add(document);
        await context.SaveChangesAsync();

        var service = new OfficialSourceService(context);

        var sources = await service.GetOfficialSourcesAsync(new DateTime(2026, 5, 26));

        Assert.Single(sources);
        Assert.Equal(new DateTime(2026, 2, 1), sources[0].LastReviewedAt);
    }
}
