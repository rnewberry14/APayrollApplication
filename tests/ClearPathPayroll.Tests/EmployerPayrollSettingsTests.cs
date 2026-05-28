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
    public void PhoneNumberFormatter_FormatsTenDigitsAndAllowsBlank()
    {
        Assert.True(PhoneNumberFormatter.TryFormat("405.555 1212", out var formatted, out var error));
        Assert.Equal("(405) 555-1212", formatted);
        Assert.Null(error);

        Assert.True(PhoneNumberFormatter.TryFormat("", out var blank, out error));
        Assert.Null(blank);
        Assert.Null(error);
    }

    [Fact]
    public void PhoneNumberFormatter_RejectsNonTenDigitInput()
    {
        var result = PhoneNumberFormatter.TryFormat("405-555", out _, out var error);

        Assert.False(result);
        Assert.Equal("Phone number must contain 10 digits.", error);
    }

    [Theory]
    [InlineData("2.5", "2.5000")]
    [InlineData("12.34567%", "12.3457")]
    public void RateFormatter_FormatsPercentRatesToFourDecimals(string input, string expected)
    {
        var result = RateFormatter.TryParsePercentRate(input, out var rate, out var error);

        Assert.True(result);
        Assert.Null(error);
        Assert.Equal(expected, RateFormatter.FormatPercentRate(rate));
    }

    [Fact]
    public void USStateList_IncludesOklahomaAndDistrictOfColumbia()
    {
        Assert.Contains(USStateList.States, state => state.Abbreviation == "OK" && state.DisplayName == "Oklahoma (OK)");
        Assert.Contains(USStateList.States, state => state.Abbreviation == "DC" && state.DisplayName == "District of Columbia (DC)");
        Assert.Equal(51, USStateList.States.Count);
    }

    [Fact]
    public void FilingFrequencyOptions_ContainRequiredDepositorTypes()
    {
        foreach (var expected in new[] { "Not Set", "Monthly", "Semiweekly", "Quarterly", "Annual", "Next-Day", "Other / User Defined" })
        {
            Assert.Contains(expected, FilingFrequencyOptions.Values);
        }
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
        company.FutaRatePlaceholder = 0.6m;
        company.LocalEmployerTaxRate = 1.23456m;
        company.SUIN = "  SUIN-OK-123  ";
        company.SEIN = "  SEIN-OK-456  ";
        company.Phone = "405 555 1212";
        company.FilingFrequencyPlaceholder = "Monthly";
        company.SutaEmployerAccountNumberPlaceholder = "suta-token-placeholder-0001";

        await service.SaveEmployerSetupAsync(company);

        var saved = await context.Companies.SingleAsync();
        Assert.Equal("OK", saved.SutaState);
        Assert.Equal(2.5m, saved.SutaRate);
        Assert.Equal(0.6m, saved.FutaRatePlaceholder);
        Assert.Equal(1.2346m, saved.LocalEmployerTaxRate);
        Assert.Equal("SUIN-OK-123", saved.SUIN);
        Assert.Equal("SEIN-OK-456", saved.SEIN);
        Assert.Equal("(405) 555-1212", saved.Phone);
        Assert.Equal("Monthly", saved.FilingFrequencyPlaceholder);
        Assert.Equal("suta-token-placeholder-0001", saved.SutaEmployerAccountNumberPlaceholder);
    }

    [Fact]
    public async Task CompanyService_SaveEmployerSetupAsync_DoesNotReplaceFeinWithDemoPlaceholder()
    {
        await using var context = CreateContext("employer_fein_save");
        var service = new CompanyService(context);
        var company = CreateCompany();
        company.FEIN = "12 3456789";

        await service.SaveEmployerSetupAsync(company);

        var saved = await context.Companies.SingleAsync();
        Assert.Equal("12-3456789", saved.FEIN);
        Assert.NotEqual("00-0000000", saved.FEIN);
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
