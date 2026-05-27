using ClearPathPayroll.Services;
using Xunit;

namespace ClearPathPayroll.Tests;

public class QuickBooksImportTemplateCatalogTests
{
    [Fact]
    public void GetTemplates_ReturnsDesktopTemplateCatalog()
    {
        var catalog = new QuickBooksImportTemplateCatalog();

        var templates = catalog.GetTemplates(QuickBooksImportSource.Desktop);

        Assert.Contains(templates, template => template.TemplateName == "Employee list");
        Assert.Contains(templates, template => template.TemplateName == "Payroll summary");
        Assert.Contains(templates, template => template.TemplateName == "Payroll item detail");
        Assert.Contains(templates, template => template.TemplateName == "Paycheck detail");
        Assert.Contains(templates, template => template.TemplateName == "Tax liability/payment report placeholder");
    }

    [Fact]
    public void GetTemplates_ReturnsOnlineTemplateCatalog()
    {
        var catalog = new QuickBooksImportTemplateCatalog();

        var templates = catalog.GetTemplates(QuickBooksImportSource.Online);

        Assert.Contains(templates, template => template.TemplateName == "Employee list");
        Assert.Contains(templates, template => template.TemplateName == "Payroll summary");
        Assert.Contains(templates, template => template.TemplateName == "Payroll details");
        Assert.Contains(templates, template => template.TemplateName == "Time activities placeholder");
        Assert.Contains(templates, template => template.TemplateName == "Tax payments placeholder");
    }

    [Fact]
    public void BuildMappings_MapsDesktopAliases()
    {
        var catalog = new QuickBooksImportTemplateCatalog();
        var template = catalog.GetTemplate(QuickBooksImportSource.Desktop, "desktop-paycheck-detail");

        var mappings = catalog.BuildMappings(template, new[] { "Employee", "Check No.", "Check Date", "Gross Pay", "Net Pay" });

        Assert.Contains(mappings, mapping => mapping.SourceColumn == "Check No." && mapping.TargetField == "Check.CheckNumber");
        Assert.Contains(mappings, mapping => mapping.SourceColumn == "Check Date" && mapping.TargetField == "Check.PayDate");
        Assert.Contains(mappings, mapping => mapping.SourceColumn == "Gross Pay" && mapping.IsRequired);
    }

    [Fact]
    public void BuildMappings_MapsOnlineAliases()
    {
        var catalog = new QuickBooksImportTemplateCatalog();
        var template = catalog.GetTemplate(QuickBooksImportSource.Online, "qbo-tax-payments");

        var mappings = catalog.BuildMappings(template, new[] { "Tax agency", "Payment date", "Payment amount", "Reference no.", "Tax type" });

        Assert.Contains(mappings, mapping => mapping.SourceColumn == "Tax agency" && mapping.TargetField == "TaxDeposit.Agency");
        Assert.Contains(mappings, mapping => mapping.SourceColumn == "Reference no." && mapping.TargetField == "TaxDeposit.ConfirmationNumber");
        Assert.Contains(mappings, mapping => mapping.SourceColumn == "Payment amount" && mapping.IsRequired);
    }

    [Fact]
    public void ValidateMappings_ReturnsMissingRequiredFields()
    {
        var catalog = new QuickBooksImportTemplateCatalog();
        var template = catalog.GetTemplate(QuickBooksImportSource.Online, "qbo-payroll-summary");
        var mappings = catalog.BuildMappings(template, new[] { "Employee", "Net pay" });

        var result = catalog.ValidateMappings(template, mappings);

        Assert.False(result.IsValid);
        Assert.Contains("Check.PayDate", result.MissingRequiredFields);
        Assert.Contains("Check.GrossPay", result.MissingRequiredFields);
    }
}
