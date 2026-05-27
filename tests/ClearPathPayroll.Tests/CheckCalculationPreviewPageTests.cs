using Xunit;

namespace ClearPathPayroll.Tests;

public class CheckCalculationPreviewPageTests
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
            "CheckCalculationPreview.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void Page_ContainsRouteInputsAndRecalculateActions()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/payroll/check-calculation/{PayrollRunId:int}\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Regular hours", content);
        Assert.Contains("Overtime hours placeholder", content);
        Assert.Contains("Additional earnings", content);
        Assert.Contains("Manual deductions", content);
        Assert.Contains("Reimbursements", content);
        Assert.Contains("Notes", content);
        Assert.Contains("Recalculate Check", content);
        Assert.Contains("Recalculate All", content);
    }

    [Fact]
    public void Page_ShowsWarningsErrorsAndSeparatePreviewApprovalPath()
    {
        var content = ReadPage();

        Assert.Contains("Errors that block approval", content);
        Assert.Contains("Warnings", content);
        Assert.Contains("Continue to Payroll Preview / Approval", content);
        Assert.DoesNotContain("Confirm Approval", content);
        Assert.DoesNotContain("Submit Direct Deposit", content);
        Assert.DoesNotContain("guaranteed compliant", content, StringComparison.OrdinalIgnoreCase);
    }
}
