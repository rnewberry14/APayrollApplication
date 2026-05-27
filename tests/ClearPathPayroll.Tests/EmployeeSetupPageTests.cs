using Xunit;

namespace ClearPathPayroll.Tests;

public class EmployeeSetupPageTests
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
            "EmployeeSetup.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void EmployeeSetupPage_ContainsRequiredRouteTabsAndActions()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/setup/employees\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Local Demo Mode:", content);
        Assert.Contains("Add New Employee", content);
        Assert.Contains("Edit Employee", content);
        Assert.Contains("Save", content);
        Assert.Contains("Cancel", content);

        foreach (var tab in new[] { "Demographics", "Employment", "Pay", "Federal W-4", "State Tax", "Direct Deposit", "User-Defined Fields", "Notes" })
        {
            Assert.Contains(tab, content);
        }
    }

    [Fact]
    public void EmployeeSetupPage_UsesLastFourAndTokenPlaceholdersOnly()
    {
        var content = ReadPage();

        Assert.Contains("SSN last 4 only", content);
        Assert.Contains("Routing token placeholder", content);
        Assert.Contains("Account token placeholder", content);
        Assert.DoesNotContain("Full SSN", content);
        Assert.DoesNotContain("Full bank account", content);
    }
}
