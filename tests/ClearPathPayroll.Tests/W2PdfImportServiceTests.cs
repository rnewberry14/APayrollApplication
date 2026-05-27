using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class W2PdfImportServiceTests
{
    [Fact]
    public void Parse_CleanTextBasedW2Sample_ExtractsLikelyValues()
    {
        var parser = new W2PdfTextParser();

        var result = parser.Parse(SampleW2Text());

        var record = Assert.Single(result.Records);
        Assert.Equal(2025, record.TaxYear);
        Assert.Equal("Demo Company LLC", record.EmployerName);
        Assert.Equal("6789", record.EmployerEINLast4Only);
        Assert.Equal("Ada", record.EmployeeFirstName);
        Assert.Equal("Lovelace", record.EmployeeLastName);
        Assert.Equal("1234", record.EmployeeSSNLast4Only);
        Assert.Equal(50000.00m, record.Box1WagesTipsOtherCompensation);
        Assert.Equal(5000.00m, record.Box2FederalIncomeTaxWithheld);
    }

    [Fact]
    public void BuildSafePreview_MasksFullSsnAndEin()
    {
        var preview = W2PdfTextParser.BuildSafePreview("SSN 123-45-6789 EIN 12-3456789");

        Assert.DoesNotContain("123-45-6789", preview);
        Assert.DoesNotContain("12-3456789", preview);
        Assert.Contains("***-**-6789", preview);
        Assert.Contains("**-***6789", preview);
    }

    [Fact]
    public void Parse_MissingFields_AddsWarningsAndLowConfidence()
    {
        var parser = new W2PdfTextParser();

        var result = parser.Parse("2025 Box 1 Wages, tips, other compensation 100.00");

        var record = Assert.Single(result.Records);
        Assert.True(record.ConfidenceScore < 0.85m);
        Assert.Contains(result.Errors, error => error.FieldName == "EmployeeName");
        Assert.Contains(result.Errors, error => error.FieldName == "EmployerName");
    }

    [Fact]
    public void Parse_ParsesCurrencyDecimals()
    {
        var parser = new W2PdfTextParser();

        var result = parser.Parse(SampleW2Text().Replace("50000.00", "$50,000.25", StringComparison.Ordinal));

        Assert.Equal(50000.25m, result.Records.Single().Box1WagesTipsOtherCompensation);
    }

    [Fact]
    public void Parse_EmptyText_ReturnsScannedPdfFallbackWarning()
    {
        var parser = new W2PdfTextParser();

        var result = parser.Parse(string.Empty);

        Assert.True(result.Records.Single().NeedsManualReview);
        Assert.Contains(result.Errors, error => error.Message.Contains("scanned or image-only", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SaveReviewedImportAsync_BlocksUntilUserConfirmed()
    {
        await using var context = CreateContext("w2_pdf_unconfirmed");
        var service = new W2PdfImportService(context, new W2PdfTextParser());
        var record = ValidRecord();
        var batch = new W2ImportBatch
        {
            FileName = "w2.pdf",
            Records = { record }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SaveReviewedImportAsync(batch, batch.Records));
    }

    [Fact]
    public async Task SaveReviewedImportAsync_SavesConfirmedRecordsWithoutFullIdentifiers()
    {
        await using var context = CreateContext("w2_pdf_confirmed");
        var service = new W2PdfImportService(context, new W2PdfTextParser());
        var record = ValidRecord();
        record.UserConfirmed = true;
        record.EmployeeSSNLast4Only = "123-45-6789";
        record.EmployerEINLast4Only = "12-3456789";
        record.RawExtractedTextPreview = "SSN 123-45-6789 EIN 12-3456789";
        var batch = new W2ImportBatch
        {
            FileName = "w2.pdf",
            Records = { record }
        };

        var saved = await service.SaveReviewedImportAsync(batch, batch.Records);

        Assert.Equal(W2ImportStatus.Confirmed, saved.ImportStatus);
        var savedRecord = await context.W2ImportRecords.SingleAsync();
        Assert.Equal("6789", savedRecord.EmployeeSSNLast4Only);
        Assert.Equal("6789", savedRecord.EmployerEINLast4Only);
        Assert.DoesNotContain("123-45-6789", savedRecord.RawExtractedTextPreview);
        Assert.DoesNotContain("12-3456789", savedRecord.RawExtractedTextPreview);
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    private static W2ImportRecord ValidRecord()
    {
        return new W2ImportRecord
        {
            TaxYear = 2025,
            EmployerName = "Demo Company LLC",
            EmployerEINLast4Only = "6789",
            EmployerAddress = "100 Local Way",
            EmployeeFirstName = "Ada",
            EmployeeLastName = "Lovelace",
            EmployeeSSNLast4Only = "1234",
            EmployeeAddress = "200 Employee St",
            Box1WagesTipsOtherCompensation = 50000m,
            RawExtractedTextPreview = "safe preview",
            ConfidenceScore = 1m
        };
    }

    private static string SampleW2Text()
    {
        return """
            2025 Form W-2
            Employer's name: Demo Company LLC Employer's address: 100 Local Way
            Employer identification number: 12-3456789
            Employee's name: Ada Lovelace Employee's address: 200 Employee St
            Social security number: 123-45-1234
            1 Wages, tips, other compensation 50000.00
            2 Federal income tax withheld 5000.00
            3 Social security wages 50000.00
            4 Social security tax withheld 3100.00
            5 Medicare wages and tips 50000.00
            6 Medicare tax withheld 725.00
            16 State wages, tips, etc. 50000.00
            17 State income tax 2000.00
            """;
    }
}
