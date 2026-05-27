using Xunit;

namespace ClearPathPayroll.Tests;

public class ImportDataPageTests
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
            "ImportData.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void ImportDataPage_ContainsRouteWorkflowAndSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/setup/imports\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Local import", content);
        Assert.Contains("Files are not uploaded to cloud storage or transmitted externally.", content);
        Assert.Contains("Demo mode warning", content);
        Assert.Contains("Validate Rows", content);
        Assert.Contains("Confirm Import", content);
    }

    [Fact]
    public void ImportDataPage_ListsSupportedImportTypes()
    {
        var content = ReadPage();

        foreach (var text in new[]
        {
            "Entire company import",
            "Employers only",
            "Employees only",
            "Employees and checks only",
            "Checks only",
            "Tax deposits only"
        })
        {
            Assert.Contains(text, content);
        }
    }
}
