using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollFieldTemplateCatalogTests
{
    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    [Fact]
    public void GetTemplates_ReturnsRequestedTemplateCatalog()
    {
        var catalog = new PayrollFieldTemplateCatalog();
        var templates = catalog.GetTemplates();

        Assert.Equal(23, templates.Count);
        Assert.Contains(templates, template => template.FieldName == "Department");
        Assert.Contains(templates, template => template.FieldName == "Job costing code");
        Assert.Contains(templates, template => template.FieldName == "Workers comp class");
        Assert.Contains(templates, template => template.FieldName == "Certified payroll classification");
        Assert.Contains(templates, template => template.FieldName == "Union code placeholder");
        Assert.Contains(templates, template => template.FieldName == "Project code");
        Assert.Contains(templates, template => template.FieldName == "Location code");
        Assert.Contains(templates, template => template.FieldName == "Tip amount");
        Assert.Contains(templates, template => template.FieldName == "Cash tips");
        Assert.Contains(templates, template => template.FieldName == "Reported tips");
        Assert.Contains(templates, template => template.FieldName == "Mileage reimbursement");
        Assert.Contains(templates, template => template.FieldName == "PTO hours");
        Assert.Contains(templates, template => template.FieldName == "Sick hours");
        Assert.Contains(templates, template => template.FieldName == "Vacation hours");
        Assert.Contains(templates, template => template.FieldName == "Bonus amount");
        Assert.Contains(templates, template => template.FieldName == "Commission amount");
        Assert.Contains(templates, template => template.FieldName == "Piecework units");
        Assert.Contains(templates, template => template.FieldName == "Shift differential");
        Assert.Contains(templates, template => template.FieldName == "Garnishment placeholder");
        Assert.Contains(templates, template => template.FieldName == "Child support withholding placeholder");
        Assert.Contains(templates, template => template.FieldName == "Health insurance deduction");
        Assert.Contains(templates, template => template.FieldName == "Retirement deduction");
        Assert.Contains(templates, template => template.FieldName == "Employer contribution placeholder");
        Assert.All(templates, template => Assert.Equal("Common optional payroll field template.", template.Description));
    }

    [Fact]
    public async Task TemplateCatalog_DoesNotActivateTemplatesWithoutServiceCall()
    {
        await using var context = CreateContext("field_templates_no_auto_activate");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();

        _ = new PayrollFieldTemplateCatalog().GetTemplates();

        Assert.Empty(await context.UserDefinedFieldDefinitions.ToListAsync());
    }

    [Fact]
    public async Task ActivateTemplateAsync_CreatesUserDefinedFieldDefinition()
    {
        await using var context = CreateContext("field_templates_activate");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();

        var catalog = new PayrollFieldTemplateCatalog();
        var template = catalog.FindTemplate("bonus-amount")!;
        var service = new UserDefinedFieldService(context);

        await service.ActivateTemplateAsync(1, template);

        var saved = await context.UserDefinedFieldDefinitions.SingleAsync();
        Assert.Equal("Bonus amount", saved.FieldName);
        Assert.Equal("BONUS_AMOUNT", saved.FieldCode);
        Assert.Equal(UserDefinedFieldDataType.Currency, saved.DataType);
        Assert.Equal(UserDefinedFieldCalculationRole.EarningAmount, saved.CalculationRole);
    }

    [Fact]
    public async Task ActivateTemplateAsync_RejectsDuplicateTemplateCode()
    {
        await using var context = CreateContext("field_templates_duplicate");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();

        var template = new PayrollFieldTemplateCatalog().FindTemplate("department")!;
        var service = new UserDefinedFieldService(context);

        await service.ActivateTemplateAsync(1, template);

        await Assert.ThrowsAsync<ValidationException>(() => service.ActivateTemplateAsync(1, template));
    }

    private static Company CreateCompany()
    {
        return new Company
        {
            CompanyId = 1,
            LegalName = "Demo Company LLC",
            FEIN = "00-0000000",
            PrimaryAddress = "100 Local Way",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            IsActive = true
        };
    }
}
