using Xunit;

namespace ClearPathPayroll.Tests;

public class EmployeesOnlyImportPageTests
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
            "EmployeesOnlyImport.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void EmployeesOnlyImportPage_ContainsRouteWorkflowAndSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/import/employees\"", content);
        Assert.Contains("@page \"/imports/employees\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("<ImportProgress", content);
        Assert.Contains("<ImportMappingPanel", content);
        Assert.Contains("Local import", content);
        Assert.Contains("No cloud upload", content);
        Assert.Contains("Do not import full SSNs", content);
        Assert.Contains("Validate Employees", content);
        Assert.Contains("Confirm Employee Import", content);
    }

    [Fact]
    public void EmployeesOnlyImportPage_ContainsRequiredAndOptionalFields()
    {
        var content = ReadPage();

        foreach (var text in new[]
        {
            "FirstName",
            "LastName",
            "FullName",
            "SSNLast4",
            "EmployeeNumber",
            "PayType",
            "HourlyRate",
            "AnnualSalary",
            "FederalFilingStatus",
            "DirectDepositLast4"
        })
        {
            Assert.Contains(text, content);
        }

        Assert.Contains("Map either First Name and Last Name, or map Full Name.", content);
    }
}
