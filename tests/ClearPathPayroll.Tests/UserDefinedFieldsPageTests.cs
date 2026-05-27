using Xunit;

namespace ClearPathPayroll.Tests;

public class UserDefinedFieldsPageTests
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
            "UserDefinedFields.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void UserDefinedFieldsPage_ContainsRouteActionsAndSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/setup/user-defined-fields\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("Optional custom field", content);
        Assert.Contains("Add Field", content);
        Assert.Contains("Save", content);
        Assert.Contains("Cancel", content);
        Assert.Contains("Executable scripts and unsafe formulas are not accepted.", content);
    }

    [Fact]
    public void UserDefinedFieldsPage_ContainsRequiredConfigurationFields()
    {
        var content = ReadPage();

        foreach (var text in new[]
        {
            "Field name",
            "Field code",
            "Applies to",
            "Data type",
            "Required",
            "Default value",
            "List options",
            "Include in payroll calculation later",
            "Calculation role",
            "Active"
        })
        {
            Assert.Contains(text, content);
        }
    }
}
