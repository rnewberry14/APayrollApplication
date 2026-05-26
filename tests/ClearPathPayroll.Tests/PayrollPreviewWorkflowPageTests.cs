using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollPreviewWorkflowPageTests
{
    private static string ReadPayrollPreviewPage()
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
            "PayrollPreview.razor");

        return File.ReadAllText(Path.GetFullPath(pagePath));
    }

    [Fact]
    public void PayrollPreview_CallsCalculationApprovalAndFakeDirectDepositServices()
    {
        var content = ReadPayrollPreviewPage();

        Assert.Contains("PayrollCalculationService.RecalculatePayrollRunAsync(PayrollRunId, PlaceholderUserId)", content);
        Assert.Contains("PayrollApprovalService.ApprovePayrollRunAsync(PayrollRunId, PlaceholderUserId)", content);
        Assert.Contains("DirectDepositSubmissionService.SubmitFakePayrollRunDirectDepositAsync(PayrollRunId, PlaceholderUserId)", content);
    }

    [Fact]
    public void PayrollPreview_GatesFakeDirectDepositWithLocalOnlyMode()
    {
        var content = ReadPayrollPreviewPage();

        Assert.Contains("IOptions<LimitedLiabilityModeOptions>", content);
        Assert.Contains("LimitedLiabilityOptions.Value.Enabled", content);
        Assert.Contains("!LimitedLiabilityOptions.Value.AllowRealAchSubmission", content);
        Assert.Contains("CanSubmitFakeDirectDeposit", content);
        Assert.Contains("Fake direct deposit is available only when Local-Only Mode is enabled and real ACH submission is disabled.", content);
    }

    [Fact]
    public void PayrollPreview_ShowsRequiredSafetyLanguage()
    {
        var content = ReadPayrollPreviewPage();

        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Local Demo Mode:", content);
        Assert.Contains("No real ACH, tax filing, or bank data transmission is enabled.", content);
        Assert.Contains("Fake batch reference", content);
        Assert.Contains("No routing numbers or full bank account numbers will be shown.", content);
    }
}
