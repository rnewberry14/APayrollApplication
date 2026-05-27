using Xunit;

namespace ClearPathPayroll.Tests;

public class HelpCenterPageTests
{
    [Fact]
    public void HelpCenterPage_ContainsRouteCategoriesSearchAndSafety()
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Pages", "HelpCenter.razor"));

        Assert.Contains("@page \"/help\"", content);
        Assert.Contains("Tester Safety Rules", content);
        Assert.Contains("Search help articles", content);
        Assert.Contains("Manual Test Packet", content);
        Assert.Contains("Tester Feedback Form", content);
        Assert.Contains("Getting Started", content);
        Assert.Contains("Setup", content);
        Assert.Contains("Payroll Processing", content);
        Assert.Contains("Reports", content);
        Assert.Contains("Imports", content);
        Assert.Contains("Official Sources", content);
        Assert.Contains("Troubleshooting", content);
        Assert.Contains("Feedback", content);
        Assert.Contains("File.ReadAllText", content);
        Assert.DoesNotContain("HttpClient", content);
    }

    [Theory]
    [InlineData("EmployeeSetup.razor", "/help?article=employee-setup")]
    [InlineData("EmployerSetup.razor", "/help?article=employer-setup")]
    [InlineData("PayrollRunCreate.razor", "/help?article=creating-payroll-run")]
    [InlineData("PayrollPreview.razor", "/help?article=payroll-preview")]
    [InlineData("PayStub.razor", "/help?article=printing-pay-stubs")]
    [InlineData("PayrollRegister.razor", "/help?article=payroll-register")]
    [InlineData("TaxLiabilityReport.razor", "/help?article=tax-liability-report")]
    [InlineData("ImportData.razor", "/help?article=importing-employees")]
    public void MajorPages_LinkToHelpCenter(string pageName, string expectedLink)
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Pages", pageName));

        Assert.Contains(expectedLink, content);
    }

    [Fact]
    public void NavMenu_LinksToHelpCenter()
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Layout", "NavMenu.razor"));

        Assert.Contains("href=\"help\"", content);
        Assert.Contains("Help Center", content);
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
