using Xunit;

namespace ClearPathPayroll.Tests;

public class TaxDepositsImportPageTests
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
            "TaxDepositsImport.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void TaxDepositsImportPage_ContainsRouteWorkflowAndSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/import/tax-deposits\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("does not submit payments", content);
        Assert.Contains("does not claim imported deposit records are verified", content);
        Assert.Contains("User-entered deposit record", content);
        Assert.Contains("Validate Tax Deposits", content);
        Assert.Contains("Save User-Entered Deposit Records", content);
    }

    [Fact]
    public void TaxDepositsImportPage_ContainsRequiredColumns()
    {
        var content = ReadPage();

        foreach (var column in new[]
        {
            "CompanyIdentifier",
            "DepositDate",
            "TaxPeriodStart",
            "TaxPeriodEnd",
            "TaxType",
            "Agency",
            "Amount",
            "ConfirmationNumber",
            "PaymentMethod",
            "Notes"
        })
        {
            Assert.Contains(column, content);
        }
    }
}
