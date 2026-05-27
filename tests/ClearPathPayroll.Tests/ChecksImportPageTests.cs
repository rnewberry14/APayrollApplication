using Xunit;

namespace ClearPathPayroll.Tests;

public class ChecksImportPageTests
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
            "ChecksImport.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void ChecksImportPage_ContainsRoutesAndSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/import/checks\"", content);
        Assert.Contains("@page \"/import/employees-and-checks\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("does not submit ACH", content);
        Assert.Contains("file taxes", content);
        Assert.Contains("Imported data is user-provided", content);
        Assert.Contains("Validate Checks", content);
        Assert.Contains("Save Draft Payroll Runs", content);
    }

    [Fact]
    public void ChecksImportPage_ContainsRequiredColumns()
    {
        var content = ReadPage();

        foreach (var column in new[]
        {
            "CompanyIdentifier",
            "EmployeeIdentifier",
            "PayDate",
            "PayPeriodStart",
            "PayPeriodEnd",
            "GrossPay",
            "RegularHours",
            "OvertimeHours",
            "EmployeeFederalTax",
            "EmployeeStateTax",
            "EmployeeLocalTax",
            "SocialSecurityTax",
            "MedicareTax",
            "Deductions",
            "NetPay",
            "PaymentMethod",
            "CheckNumber",
            "DirectDepositLast4"
        })
        {
            Assert.Contains(column, content);
        }
    }
}
