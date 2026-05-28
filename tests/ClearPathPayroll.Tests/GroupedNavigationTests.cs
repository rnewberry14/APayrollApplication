using Xunit;

namespace ClearPathPayroll.Tests;

public class GroupedNavigationTests
{
    [Fact]
    public void NavMenu_UsesTesterFriendlyGroupedCategories()
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Layout", "NavMenu.razor"));

        foreach (var category in new[]
        {
            "Home",
            "Help Center",
            "Payer/Employer",
            "Employees",
            "Payroll",
            "Imports Center"
        })
        {
            Assert.Contains($"> {category}</summary>", content);
        }

        Assert.Contains("<details class=\"nav-section\"", content);
        Assert.Contains("<details class=\"nav-subsection\"", content);
        Assert.Contains("Import from QuickBooks", content);
    }

    [Theory]
    [InlineData("href=\"\"")]
    [InlineData("href=\"help\"")]
    [InlineData("href=\"prototype-test-checklist\"")]
    [InlineData("href=\"readme\"")]
    [InlineData("href=\"demo/seed-data\"")]
    [InlineData("href=\"demo/reset-data\"")]
    [InlineData("href=\"employers/select\"")]
    [InlineData("href=\"employers/new\"")]
    [InlineData("href=\"employers/edit\"")]
    [InlineData("href=\"employers/payroll-settings\"")]
    [InlineData("href=\"setup/user-defined-fields\"")]
    [InlineData("href=\"employees/edit\"")]
    [InlineData("href=\"employees/new\"")]
    [InlineData("href=\"setup/pay-schedules\"")]
    [InlineData("href=\"payroll/create\"")]
    [InlineData("href=\"payroll/print\"")]
    [InlineData("href=\"payroll/view\"")]
    [InlineData("href=\"imports/employer-employee\"")]
    [InlineData("href=\"imports/employees-checks\"")]
    [InlineData("href=\"imports/employees\"")]
    [InlineData("href=\"imports/paychecks\"")]
    [InlineData("href=\"imports/tax-deposits\"")]
    [InlineData("href=\"imports/quickbooks/desktop\"")]
    [InlineData("href=\"imports/quickbooks/online\"")]
    [InlineData("href=\"imports/w2\"")]
    public void NavMenu_LinksRequestedRoutes(string expectedLink)
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Layout", "NavMenu.razor"));

        Assert.Contains(expectedLink, content);
    }

    [Theory]
    [InlineData("EmployerSetup.razor", "@page \"/employers/select\"")]
    [InlineData("EmployerSetup.razor", "@page \"/employers/new\"")]
    [InlineData("EmployerSetup.razor", "@page \"/employers/edit\"")]
    [InlineData("EmployerPayrollSettings.razor", "@page \"/employers/payroll-settings\"")]
    [InlineData("EmployeeSetup.razor", "@page \"/employees/edit\"")]
    [InlineData("EmployeeSetup.razor", "@page \"/employees/new\"")]
    [InlineData("ImportData.razor", "@page \"/imports/employer-employee\"")]
    [InlineData("EmployeesOnlyImport.razor", "@page \"/imports/employees\"")]
    [InlineData("ChecksImport.razor", "@page \"/imports/paychecks\"")]
    [InlineData("ChecksImport.razor", "@page \"/imports/employees-checks\"")]
    [InlineData("TaxDepositsImport.razor", "@page \"/imports/tax-deposits\"")]
    [InlineData("QuickBooksImport.razor", "@page \"/imports/quickbooks/desktop\"")]
    [InlineData("QuickBooksImport.razor", "@page \"/imports/quickbooks/online\"")]
    [InlineData("W2Import.razor", "@page \"/imports/w2\"")]
    public void RequestedRoutes_AliasExistingPagesWherePossible(string pageName, string expectedRoute)
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Pages", pageName));

        Assert.Contains(expectedRoute, content);
    }

    [Fact]
    public void PlaceholderPage_ClearlyLabelsTestingPlaceholders()
    {
        var content = File.ReadAllText(RepoPath("src", "ClearPathPayroll", "Components", "Pages", "NavigationPlaceholder.razor"));

        Assert.Contains("@page \"/readme\"", content);
        Assert.Contains("@page \"/demo/reset-data\"", content);
        Assert.Contains("@page \"/payroll/print\"", content);
        Assert.Contains("@page \"/payroll/view\"", content);
        Assert.Contains("Feature placeholder for testing.", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
    }

    private static string RepoPath(params string[] parts)
    {
        var root = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            ".."));

        return Path.Combine(new[] { root }.Concat(parts).ToArray());
    }
}
