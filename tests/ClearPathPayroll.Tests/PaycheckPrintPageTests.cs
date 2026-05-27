using Xunit;

namespace ClearPathPayroll.Tests;

public class PaycheckPrintPageTests
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
            "PaycheckPrint.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void Page_ContainsRoutePrintButtonWatermarkAndDisclaimer()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/payroll/paycheck/{PayrollRunEmployeeId:int}\"", content);
        Assert.Contains("Print Paycheck", content);
        Assert.Contains("NON-NEGOTIABLE / DEMO", content);
        Assert.Contains("Printed checks are user-controlled documents", content);
        Assert.Contains("@@media print", content);
        Assert.Contains("Tracking placeholder", content);
    }

    [Fact]
    public void Page_DoesNotPrintSensitiveOrBankApprovalLanguage()
    {
        var content = ReadPage();

        Assert.DoesNotContain("full SSN", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("bank account number is", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("does not create bank-positive-pay files", content);
        Assert.Contains("does not create bank-positive-pay files, submit ACH, or imply bank approval", content);
    }
}
