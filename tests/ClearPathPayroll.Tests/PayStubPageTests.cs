using Xunit;

namespace ClearPathPayroll.Tests;

public class PayStubPageTests
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
            "PayStub.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void PayStubPage_ContainsNewRouteDisclaimerAndRequiredSections()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/payroll/paystub/{PayrollRunEmployeeId:int}\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Review applicable pay statement requirements before distribution.", content);
        Assert.Contains("Company", content);
        Assert.Contains("Employee ID", content);
        Assert.Contains("SSN last 4", content);
        Assert.Contains("Earnings Detail", content);
        Assert.Contains("Employee Taxes", content);
        Assert.Contains("Employer Taxes Optional/Internal Section", content);
        Assert.Contains("Pre-Tax Deductions", content);
        Assert.Contains("Post-Tax Deductions", content);
        Assert.Contains("Reimbursements", content);
        Assert.Contains("Year-to-Date Placeholders", content);
        Assert.Contains("Employer Notes", content);
    }

    [Fact]
    public void PayStubPage_ContainsConfigurableSettingsAndPrintCss()
    {
        var content = ReadPage();

        Assert.Contains("Show hours", content);
        Assert.Contains("Show rates", content);
        Assert.Contains("Show YTD", content);
        Assert.Contains("Show employer taxes", content);
        Assert.Contains("Show employee address", content);
        Assert.Contains("Show accrual placeholders", content);
        Assert.Contains("State-required-fields checklist placeholder", content);
        Assert.Contains("@@media print", content);
        Assert.Contains("Print Pay Stub", content);
    }

    [Fact]
    public void PayStubPage_DoesNotContainComplianceGuaranteeLanguage()
    {
        var content = ReadPage();

        Assert.DoesNotContain("legally compliant", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("guaranteed compliant", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ensures compliance", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("full SSN", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("full bank account", content, StringComparison.OrdinalIgnoreCase);
    }
}
