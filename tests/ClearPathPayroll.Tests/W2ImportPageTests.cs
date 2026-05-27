using Xunit;

namespace ClearPathPayroll.Tests;

public class W2ImportPageTests
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
            "W2Import.razor");

        return File.ReadAllText(Path.GetFullPath(path));
    }

    [Fact]
    public void W2ImportPage_ContainsRouteWorkflowAndSafetyText()
    {
        var content = ReadPage();

        Assert.Contains("@page \"/import/w2\"", content);
        Assert.Contains("<NoAdviceDisclaimer />", content);
        Assert.Contains("does not OCR PDF forms", content);
        Assert.Contains("does not", content);
        Assert.Contains("create tax filings", content);
        Assert.Contains("historical user-entered data", content);
        Assert.Contains("Validate W-2 Rows", content);
        Assert.Contains("Save Historical W-2 Records", content);
    }

    [Fact]
    public void W2ImportPage_ContainsRequiredColumns()
    {
        var content = ReadPage();

        foreach (var column in new[]
        {
            "TaxYear",
            "EmployerName",
            "EmployerEIN",
            "EmployerAddress",
            "EmployeeFirstName",
            "EmployeeLastName",
            "EmployeeSSNLast4",
            "EmployeeAddress",
            "Box1Wages",
            "Box2FederalTaxWithheld",
            "Box3SocialSecurityWages",
            "Box4SocialSecurityTaxWithheld",
            "Box5MedicareWages",
            "Box6MedicareTaxWithheld",
            "Box12CodeAndAmount",
            "Box14DescriptionAndAmount",
            "StateWages",
            "StateTaxWithheld",
            "LocalWages",
            "LocalTaxWithheld"
        })
        {
            Assert.Contains(column, content);
        }
    }
}
