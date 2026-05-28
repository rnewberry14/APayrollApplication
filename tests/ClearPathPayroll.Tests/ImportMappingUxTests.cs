using ClearPathPayroll.Services;
using Xunit;

namespace ClearPathPayroll.Tests;

public class ImportMappingUxTests
{
    [Theory]
    [InlineData("First Name", "FirstName")]
    [InlineData("first_name", "FirstName")]
    [InlineData("Employee First", "FirstName")]
    [InlineData("Last Name", "LastName")]
    [InlineData("Full Name", "FullName")]
    [InlineData("Worker Name", "FullName")]
    [InlineData("SSN Last 4", "SSNLast4")]
    [InlineData("DOB", "DateOfBirth")]
    [InlineData("Hire Date", "HireDate")]
    [InlineData("Hourly Rate", "HourlyRate")]
    [InlineData("Salary", "AnnualSalary")]
    public void AliasMatcher_MatchesCommonColumnNames(string sourceColumn, string expectedTarget)
    {
        var targetFields = new[]
        {
            "FirstName",
            "LastName",
            "FullName",
            "SSNLast4",
            "DateOfBirth",
            "HireDate",
            "HourlyRate",
            "AnnualSalary"
        };

        var suggestion = ImportFieldAliasMatcher.SuggestTargetField(sourceColumn, targetFields);

        Assert.Equal(expectedTarget, suggestion);
    }

    [Fact]
    public void AliasMatcher_IgnoresSpacesUnderscoresDashesCaseAndPunctuation()
    {
        var normalizedA = ImportFieldAliasMatcher.Normalize("Employee_First-Name.");
        var normalizedB = ImportFieldAliasMatcher.Normalize("employee first name");

        Assert.Equal(normalizedA, normalizedB);
    }

    [Fact]
    public void RequiredUnmappedCount_CountsMissingRequiredTargets()
    {
        var mappings = new List<ImportMappingInput>
        {
            new() { SourceColumn = "First Name", TargetField = "FirstName" },
            new() { SourceColumn = "Last Name", TargetField = "" },
            new() { SourceColumn = "Pay Type", TargetField = "PayType" }
        };

        var count = ImportFieldAliasMatcher.CountUnmappedRequiredFields(
            new[] { "FirstName", "LastName", "PayType" },
            mappings);

        Assert.Equal(1, count);
    }

    [Fact]
    public void AliasMatcher_HandlesLargeColumnSet()
    {
        var targetFields = new[] { "FirstName", "LastName", "HourlyRate" };
        var columns = Enumerable.Range(1, 60)
            .Select(index => $"Unused Column {index}")
            .Append("Hourly Rate")
            .ToList();

        var suggestions = columns.Select(column => ImportFieldAliasMatcher.SuggestTargetField(column, targetFields)).ToList();

        Assert.Equal(61, suggestions.Count);
        Assert.Contains("HourlyRate", suggestions);
    }

    [Fact]
    public void ImportProgressModel_TracksStageStatusAndRows()
    {
        var progress = new ImportProgressModel();

        Assert.Equal("Not started", progress.StatusLabel);

        progress.MoveTo(ImportProgressStage.ParsingRows, "Parsing rows.", rowCount: 25);
        Assert.Equal("Working", progress.StatusLabel);
        Assert.True(progress.IsWorking);
        Assert.Equal(25, progress.RowCount);

        progress.MoveTo(ImportProgressStage.PreviewReady, "Review mappings.", errorCount: 0);
        Assert.Equal("Needs review", progress.StatusLabel);

        progress.MoveTo(ImportProgressStage.Complete, "Done.");
        Assert.Equal("Complete", progress.StatusLabel);
        Assert.Equal(100, progress.PercentComplete);
    }

    [Fact]
    public void MappingPanel_ContainsTemplatePlaceholderAndFilterControls()
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Shared", "ImportMappingPanel.razor"));

        Assert.Contains("Search source columns", content);
        Assert.Contains("Search target fields", content);
        Assert.Contains("Show unmapped only", content);
        Assert.Contains("Show required only", content);
        Assert.Contains("Auto-map suggested fields", content);
        Assert.Contains("Clear all mappings", content);
        Assert.Contains("Save mapping template", content);
        Assert.Contains("Load mapping template", content);
        Assert.Contains("Mapping template save is a placeholder for testing.", content);
        Assert.Contains("sticky-summary", content);
    }

    [Fact]
    public void ErrorMessageSanitizer_DoesNotExposeLongRawValues()
    {
        var longMessage = $"Invalid value: {new string('9', 300)}";

        var sanitized = ImportFieldAliasMatcher.SanitizeErrorMessage(longMessage);

        Assert.True(sanitized.Length <= 183);
        Assert.DoesNotContain(new string('9', 200), sanitized);
    }

    private static string RepoPath(params string[] parts)
    {
        var root = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            ".."));

        return Path.Combine(new[] { root }.Concat(parts).ToArray());
    }
}
