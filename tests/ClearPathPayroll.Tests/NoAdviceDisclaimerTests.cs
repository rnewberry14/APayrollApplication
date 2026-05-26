using Xunit;

namespace ClearPathPayroll.Tests;

public class NoAdviceDisclaimerTests
{
    private const string RequiredDisclaimer = "ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.";

    [Fact]
    public void NoAdviceDisclaimer_ComponentContainsRequiredDisclaimer()
    {
        var componentPath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "src",
            "ClearPathPayroll",
            "Components",
            "Shared",
            "NoAdviceDisclaimer.razor");

        var content = File.ReadAllText(Path.GetFullPath(componentPath));

        Assert.Contains(RequiredDisclaimer, content);
    }

    [Theory]
    [InlineData("PayrollPreview.razor")]
    [InlineData("TaxLiabilityReport.razor")]
    public void RequiredPages_RenderNoAdviceDisclaimer(string fileName)
    {
        var pagePath = Path.Combine(
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
            fileName);

        var content = File.ReadAllText(Path.GetFullPath(pagePath));

        Assert.Contains("<NoAdviceDisclaimer />", content);
    }
}
