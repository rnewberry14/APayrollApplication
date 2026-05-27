using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace ClearPathPayroll.Tests;

public class TaxDepositsImportServiceTests
{
    [Fact]
    public async Task ConfirmAsync_CreatesUserEnteredDepositRecords()
    {
        await using var context = CreateContext("tax_deposit_import_confirm");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv());
        var parseResult = await service.ParseFileAsync(stream, "tax-deposits.csv");

        var validation = await service.ValidateAsync(
            "tax-deposits.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            reviewAcknowledged: true);

        Assert.True(validation.CanImport);

        var confirmation = await service.ConfirmAsync(validation.Batch.ImportBatchId);

        Assert.Equal(1, confirmation.ImportedDepositCount);
        var deposit = await context.TaxDepositRecords.SingleAsync();
        Assert.Equal("User-entered deposit record", deposit.RecordSource);
        Assert.Equal("Federal withholding", deposit.TaxType);
        Assert.Equal(250m, deposit.Amount);
    }

    [Fact]
    public async Task ValidateAsync_RejectsUnknownCompany()
    {
        await using var context = CreateContext("tax_deposit_import_unknown_company");
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv());
        var parseResult = await service.ParseFileAsync(stream, "tax-deposits.csv");

        var validation = await service.ValidateAsync(
            "tax-deposits.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            reviewAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "CompanyNotFound");
    }

    [Fact]
    public async Task ValidateAsync_RejectsInvalidPeriodAndAmount()
    {
        await using var context = CreateContext("tax_deposit_import_invalid_period");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        var csv = ValidCsv()
            .Replace("2026-05-01,2026-05-15", "2026-05-15,2026-05-01", StringComparison.Ordinal)
            .Replace("250.00", "0.00", StringComparison.Ordinal);
        await using var stream = ToStream(csv);
        var parseResult = await service.ParseFileAsync(stream, "tax-deposits.csv");

        var validation = await service.ValidateAsync(
            "tax-deposits.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            reviewAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "InvalidTaxPeriod");
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "InvalidAmount");
    }

    [Fact]
    public async Task ConfirmAsync_BlocksWhenValidationFails()
    {
        await using var context = CreateContext("tax_deposit_import_blocked");
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv());
        var parseResult = await service.ParseFileAsync(stream, "tax-deposits.csv");
        var validation = await service.ValidateAsync("tax-deposits.csv", parseResult, CreateMappings(parseResult.Columns), true);

        await Assert.ThrowsAsync<ValidationException>(() => service.ConfirmAsync(validation.Batch.ImportBatchId));
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    private static TaxDepositsImportService CreateService(PayrollDbContext context)
    {
        var importService = new ImportService(context, new IImportFileParser[]
        {
            new CsvImportFileParser(),
            new TabDelimitedImportFileParser(),
            new ExcelImportFileParser()
        });

        return new TaxDepositsImportService(context, importService);
    }

    private static IEnumerable<ImportMappingInput> CreateMappings(IEnumerable<string> columns)
    {
        return columns.Select(column => new ImportMappingInput
        {
            SourceColumn = column,
            TargetField = column
        });
    }

    private static string ValidCsv()
    {
        return "CompanyIdentifier,DepositDate,TaxPeriodStart,TaxPeriodEnd,TaxType,Agency,Amount,ConfirmationNumber,PaymentMethod,Notes\n" +
            "Demo Company LLC,2026-05-20,2026-05-01,2026-05-15,Federal withholding,IRS,250.00,CONF-123,EFTPS,Imported historical record";
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

    private static MemoryStream ToStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}
