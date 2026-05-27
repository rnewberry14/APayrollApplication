using Xunit;

namespace ClearPathPayroll.Tests;

public class EmployerSetupPageTests
{
    private static string ReadPage(string fileName)
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
            fileName);

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void EmployerSetupPage_ContainsRouteActionsAndSafetyText()
    {
        var content = ReadPage("EmployerSetup.razor");

        Assert.Contains("@page \"/setup/employer\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Local Demo Mode:", content);
        Assert.Contains("Add New Employer", content);
        Assert.Contains("Edit Employer", content);
        Assert.Contains("Save", content);
        Assert.Contains("Cancel", content);
        Assert.Contains("MaskFEIN", content);
    }

    [Fact]
    public void EmployerPayrollSettingsPage_ContainsPayrollItemAndNeutralRateText()
    {
        var content = ReadPage("EmployerPayrollSettings.razor");

        Assert.Contains("@page \"/setup/employer/payroll-settings\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("User-entered rate", content);
        Assert.Contains("Verify with official agency records", content);
        Assert.Contains("Add New Payroll Item", content);
        Assert.Contains("Save Payroll Settings", content);
        Assert.Contains("Save Item", content);
        Assert.Contains("MaskAccountPlaceholder", content);
    }

    [Fact]
    public void EmployerPages_DoNotContainComplianceGuaranteeLanguage()
    {
        var combined = ReadPage("EmployerSetup.razor") + ReadPage("EmployerPayrollSettings.razor");

        Assert.DoesNotContain("guaranteed compliant", combined, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ensures compliance", combined, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("advises", combined, StringComparison.OrdinalIgnoreCase);
    }
}
