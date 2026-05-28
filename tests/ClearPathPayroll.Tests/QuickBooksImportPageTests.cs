using Xunit;

namespace ClearPathPayroll.Tests;

public class QuickBooksImportPageTests
{
    private static string ReadPage()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "src",
            "ClearPathPayroll",
            "Components",
            "Pages",
            "QuickBooksImport.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void QuickBooksImportPage_ContainsRoutesAndSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/import/quickbooks-desktop\"", content);
        Assert.Contains("@page \"/import/quickbooks-online\"", content);
        Assert.Contains("@page \"/imports/quickbooks/desktop\"", content);
        Assert.Contains("@page \"/imports/quickbooks/online\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("<ImportProgress", content);
        Assert.Contains("<ImportMappingPanel", content);
        Assert.Contains("does not connect to QuickBooks", content);
        Assert.Contains("store Intuit credentials", content);
        Assert.Contains("upload files", content);
        Assert.Contains("Confirm Local Import Batch", content);
    }

    [Fact]
    public void QuickBooksImportPage_ContainsInstructionsAndWorkflow()
    {
        var content = ReadPage();

        Assert.Contains("QuickBooks Desktop Export Instructions", content);
        Assert.Contains("QuickBooks Online Export Instructions", content);
        Assert.Contains("Export the report to Excel or CSV", content);
        Assert.Contains("Save the exported file locally", content);
        Assert.Contains("Map columns", content);
        Assert.Contains("Validate Mappings", content);
        Assert.Contains("/imports/quickbooks/online", content);
    }
}
