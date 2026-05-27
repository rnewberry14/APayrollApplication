using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ClearPathPayroll.Tests;

public class EmployerPayrollSettingsTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    [Fact]
    public void CompanyValidation_RejectsPlainAccountNumberPlaceholders()
    {
        var company = CreateCompany();
        company.SutaEmployerAccountNumberPlaceholder = "123456789";

        var results = Validate(company);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(Company.SutaEmployerAccountNumberPlaceholder)));
    }

    [Fact]
    public void CompanyService_MaskAccountPlaceholder_HidesEnteredValue()
    {
        var masked = CompanyService.MaskAccountPlaceholder("token-account-reference-6789");

        Assert.Equal("****6789", masked);
    }

    [Fact]
    public void PayrollItemValidation_RejectsEndDateBeforeEffectiveDate()
    {
        var item = CreatePayrollItem();
        item.EffectiveDate = new DateTime(2026, 5, 26);
        item.EndDate = new DateTime(2026, 5, 25);

        var results = Validate(item);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(PayrollItem.EndDate)));
    }

    [Fact]
    public async Task PayrollItemService_SavePayrollItemAsync_PersistsItemLocally()
    {
        await using var context = CreateContext("payroll_item_save");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();

        var service = new PayrollItemService(context);
        var item = CreatePayrollItem();

        await service.SavePayrollItemAsync(item);

        var saved = await context.PayrollItems.SingleAsync();
        Assert.Equal("REG", saved.ItemCode);
        Assert.Equal(PayrollItemType.Earnings, saved.ItemType);
    }

    [Fact]
    public async Task CompanyService_SaveEmployerSetupAsync_PersistsEmployerTaxSettings()
    {
        await using var context = CreateContext("employer_settings_save");
        var service = new CompanyService(context);
        var company = CreateCompany();
        company.SutaState = "OK";
        company.SutaRate = 2.5m;
        company.SutaEmployerAccountNumberPlaceholder = "suta-token-placeholder-0001";

        await service.SaveEmployerSetupAsync(company);

        var saved = await context.Companies.SingleAsync();
        Assert.Equal("OK", saved.SutaState);
        Assert.Equal(2.5m, saved.SutaRate);
        Assert.Equal("suta-token-placeholder-0001", saved.SutaEmployerAccountNumberPlaceholder);
    }

    private static Company CreateCompany()
    {
        return new Company
        {
            LegalName = "Demo Company LLC",
            FEIN = "00-0000000",
            PrimaryAddress = "100 Local Way",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            IsActive = true
        };
    }

    private static PayrollItem CreatePayrollItem()
    {
        return new PayrollItem
        {
            CompanyId = 1,
            ItemCode = "REG",
            ItemName = "Regular Earnings",
            ItemType = PayrollItemType.Earnings,
            CalculationType = PayrollItemCalculationType.HourlyRate,
            DefaultRate = 25m,
            EffectiveDate = new DateTime(2026, 5, 26),
            IsActive = true
        };
    }
}
