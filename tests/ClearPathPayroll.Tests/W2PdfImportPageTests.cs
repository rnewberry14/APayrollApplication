using Xunit;

namespace ClearPathPayroll.Tests;

public class W2PdfImportPageTests
{
    [Fact]
    public void W2PdfImportPage_ContainsRouteWorkflowAndSafetyText()
    {
        var content = File.ReadAllText(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            "..",
            "src",
            "ClearPathPayroll",
            "Components",
            "Pages",
            "W2PdfImport.razor"));

        Assert.Contains("@page \"/import/w2-pdf\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Parse PDF", content);
        Assert.Contains("Save Reviewed Records", content);
        Assert.Contains("Cancel Import", content);
        Assert.Contains("Clear Import", content);
        Assert.Contains("does not verify, validate, or guarantee", content);
        Assert.Contains("I reviewed the extracted W-2 information", content);
        Assert.Contains("Original PDF files are not stored", content);
    }
}
