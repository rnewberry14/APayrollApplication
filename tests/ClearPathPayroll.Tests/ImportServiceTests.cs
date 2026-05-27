using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace ClearPathPayroll.Tests;

public class ImportServiceTests
{
    [Fact]
    public async Task CreateStagedBatchAsync_ValidatesAndConfirmsLocalRows()
    {
        await using var context = CreateContext("import_service_confirm");
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName\nAda,Lovelace");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var batch = await service.CreateStagedBatchAsync(
            "employees.csv",
            ImportType.EmployeesOnly,
            parseResult,
            new[]
            {
                new ImportMappingInput { SourceColumn = "FirstName", TargetField = "Employee.FirstName", IsRequired = true },
                new ImportMappingInput { SourceColumn = "LastName", TargetField = "Employee.LastName", IsRequired = true }
            },
            demoModeWarningAcknowledged: true);

        Assert.Equal(ImportBatchStatus.ReadyForConfirmation, batch.Status);
        Assert.Equal(1, batch.ValidRowCount);
        Assert.Equal(0, batch.ErrorCount);

        var confirmed = await service.ConfirmImportAsync(batch.ImportBatchId);

        Assert.Equal(ImportBatchStatus.Imported, confirmed.Status);
        Assert.All(confirmed.Rows, row => Assert.Equal(ImportRowStatus.Imported, row.Status));
    }

    [Fact]
    public async Task CreateStagedBatchAsync_BlocksConfirmationWhenRequiredValueMissing()
    {
        await using var context = CreateContext("import_service_missing_required");
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName\nAda,");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var batch = await service.CreateStagedBatchAsync(
            "employees.csv",
            ImportType.EmployeesOnly,
            parseResult,
            new[]
            {
                new ImportMappingInput { SourceColumn = "LastName", TargetField = "Employee.LastName", IsRequired = true }
            },
            demoModeWarningAcknowledged: true);

        Assert.Equal(ImportBatchStatus.ValidationFailed, batch.Status);
        Assert.Equal(1, batch.ErrorCount);
        await Assert.ThrowsAsync<ValidationException>(() => service.ConfirmImportAsync(batch.ImportBatchId));
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    private static ImportService CreateService(PayrollDbContext context)
    {
        return new ImportService(context, new IImportFileParser[]
        {
            new CsvImportFileParser(),
            new TabDelimitedImportFileParser(),
            new ExcelImportFileParser()
        });
    }

    private static MemoryStream ToStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}
