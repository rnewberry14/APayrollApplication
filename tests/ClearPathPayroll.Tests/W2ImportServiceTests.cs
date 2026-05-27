using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace ClearPathPayroll.Tests;

public class W2ImportServiceTests
{
    [Fact]
    public async Task ConfirmAsync_CreatesHistoricalUserEnteredDataWithMaskedEin()
    {
        await using var context = CreateContext("w2_import_confirm");
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv());
        var parseResult = await service.ParseFileAsync(stream, "w2.csv");

        var validation = await service.ValidateAsync("w2.csv", parseResult, CreateMappings(parseResult.Columns), reviewAcknowledged: true);

        Assert.True(validation.CanImport);

        var confirmation = await service.ConfirmAsync(validation.Batch.ImportBatchId);

        Assert.Equal(1, confirmation.ImportedRecordCount);
        var record = await context.W2HistoricalRecords.SingleAsync();
        Assert.Equal("historical user-entered data", record.RecordSource);
        Assert.Equal("**-***6789", record.EmployerEINMasked);
        Assert.Equal("1234", record.EmployeeSSNLast4);
        Assert.Equal(50000m, record.Box1Wages);
    }

    [Fact]
    public async Task ValidateAsync_RejectsFullSsn()
    {
        await using var context = CreateContext("w2_import_full_ssn");
        var service = CreateService(context);
        var csv = ValidCsv()
            .Replace("EmployeeSSNLast4", "SSN", StringComparison.Ordinal)
            .Replace("1234", "123456789", StringComparison.Ordinal);
        await using var stream = ToStream(csv);
        var parseResult = await service.ParseFileAsync(stream, "w2.csv");
        var mappings = CreateMappings(parseResult.Columns).Select(mapping =>
        {
            if (mapping.SourceColumn == "SSN")
            {
                mapping.TargetField = "EmployeeSSNLast4";
            }

            return mapping;
        });

        var validation = await service.ValidateAsync("w2.csv", parseResult, mappings, reviewAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "InvalidEmployeeSSNLast4");
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "FullSsnColumnMapped");
    }

    [Fact]
    public async Task ValidateAsync_RejectsNegativeAmounts()
    {
        await using var context = CreateContext("w2_import_negative");
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv().Replace("50000.00", "-1.00", StringComparison.Ordinal));
        var parseResult = await service.ParseFileAsync(stream, "w2.csv");

        var validation = await service.ValidateAsync("w2.csv", parseResult, CreateMappings(parseResult.Columns), reviewAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "NegativeAmount");
    }

    [Fact]
    public async Task ConfirmAsync_BlocksWhenValidationFails()
    {
        await using var context = CreateContext("w2_import_blocked");
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv().Replace("1234", "123456789", StringComparison.Ordinal));
        var parseResult = await service.ParseFileAsync(stream, "w2.csv");
        var validation = await service.ValidateAsync("w2.csv", parseResult, CreateMappings(parseResult.Columns), reviewAcknowledged: true);

        await Assert.ThrowsAsync<ValidationException>(() => service.ConfirmAsync(validation.Batch.ImportBatchId));
    }

    [Fact]
    public void MaskEin_MasksFullEin()
    {
        Assert.Equal("**-***6789", W2ImportService.MaskEin("12-3456789"));
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    private static W2ImportService CreateService(PayrollDbContext context)
    {
        var importService = new ImportService(context, new IImportFileParser[]
        {
            new CsvImportFileParser(),
            new TabDelimitedImportFileParser(),
            new ExcelImportFileParser()
        });

        return new W2ImportService(context, importService);
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
        return "TaxYear,EmployerName,EmployerEIN,EmployerAddress,EmployeeFirstName,EmployeeLastName,EmployeeSSNLast4,EmployeeAddress,Box1Wages,Box2FederalTaxWithheld,Box3SocialSecurityWages,Box4SocialSecurityTaxWithheld,Box5MedicareWages,Box6MedicareTaxWithheld,Box12CodeAndAmount,Box14DescriptionAndAmount,StateWages,StateTaxWithheld,LocalWages,LocalTaxWithheld\n" +
            "2025,Demo Company LLC,12-3456789,100 Local Way,Ada,Lovelace,1234,200 Employee St,50000.00,5000.00,50000.00,3100.00,50000.00,725.00,D 100.00,Local 50.00,50000.00,2000.00,10000.00,100.00";
    }

    private static MemoryStream ToStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}
