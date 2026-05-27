using Xunit;

namespace ClearPathPayroll.Tests;

public class FieldTemplatesPageTests
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
            "FieldTemplates.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void FieldTemplatesPage_ContainsRouteAndRequiredSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/setup/field-templates\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Common optional payroll field template", content);
        Assert.Contains("Templates are not activated automatically", content);
        Assert.Contains("Activate", content);
        Assert.Contains("Edit Activated Fields", content);
        Assert.Contains("does not run formulas", content);
    }

    [Fact]
    public void FieldTemplatesPage_DoesNotUseRiskyAdviceLanguage()
    {
        var content = ReadPage();

        Assert.DoesNotContain("legally required", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("guaranteed compliant", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ensures compliance", content, StringComparison.OrdinalIgnoreCase);
    }
}
