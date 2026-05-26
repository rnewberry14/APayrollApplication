using Xunit;

namespace ClearPathPayroll.Tests;

public class PrototypeTestChecklistPageTests
{
    [Fact]
    public void PrototypeChecklistPage_HasRoutePrototypeGuardAndDisclaimer()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/prototype-test-checklist\"", content);
        Assert.Contains("PrototypeModeHelper.ShouldUseLocalPrototypeMode", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("This page does not transmit data externally", content);
    }

    [Theory]
    [InlineData("Confirm Local Prototype Mode is enabled")]
    [InlineData("Seed demo data")]
    [InlineData("Open Company Setup")]
    [InlineData("Open Employee Setup")]
    [InlineData("Open Pay Schedule Setup")]
    [InlineData("Create Payroll Run")]
    [InlineData("Calculate Payroll")]
    [InlineData("Preview Payroll")]
    [InlineData("Approve Payroll")]
    [InlineData("Submit Fake Direct Deposit")]
    [InlineData("Open Pay Stub")]
    [InlineData("Open Payroll Register")]
    [InlineData("Open Tax Liability Report")]
    [InlineData("Export CSV reports")]
    [InlineData("Confirm no real ACH was submitted")]
    [InlineData("Confirm no tax filing was submitted")]
    [InlineData("Confirm no full SSN or bank account number is displayed")]
    public void PrototypeChecklistPage_IncludesRequiredChecklistStep(string expectedStep)
    {
        var content = ReadPage();

        Assert.Contains(expectedStep, content);
    }

    private static string ReadPage()
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
            "PrototypeTestChecklist.razor");

        return File.ReadAllText(Path.GetFullPath(pagePath));
    }
}
