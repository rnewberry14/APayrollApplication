using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollRunWizardPageTests
{
    [Fact]
    public void PayrollRunWizardPage_ContainsRequiredRouteBannersAndSteps()
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
            "PayrollRunCreate.razor");

        var content = File.ReadAllText(Path.GetFullPath(pagePath));

        Assert.Contains("@page \"/payroll/create\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Local Demo Mode", content);
        Assert.Contains("Step 1: Select Company", content);
        Assert.Contains("Step 2: Select Pay Schedule", content);
        Assert.Contains("Step 3: Select Payroll Mode", content);
        Assert.Contains("Step 4: Confirm Payroll Dates", content);
        Assert.Contains("Step 5: Select Employees", content);
        Assert.Contains("Step 6: Enter Pay Data", content);
        Assert.Contains("Regular payroll", content);
        Assert.Contains("After-the-fact payroll", content);
        Assert.Contains("Correction payroll", content);
        Assert.Contains("Overtime hours placeholder", content);
        Assert.Contains("Manual gross adjustment", content);
        Assert.Contains("Notes", content);
    }
}
