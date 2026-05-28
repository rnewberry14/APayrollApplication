using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace ClearPathPayroll.Tests;

public class EmployeesOnlyImportServiceTests
{
    [Fact]
    public async Task ConfirmAsync_CreatesEmployeesAfterValidation()
    {
        await using var context = CreateContext("employees_import_confirm");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName,EmployeeNumber,PayType,HourlyRate,SSNLast4\nAda,Lovelace,E001,Hourly,25.50,1234");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        Assert.True(validation.CanImport);

        var confirmation = await service.ConfirmAsync(1, validation.Batch.ImportBatchId);

        Assert.Equal(1, confirmation.ImportedEmployeeCount);
        var employee = await context.Employees.SingleAsync();
        Assert.Equal("Ada", employee.FirstName);
        Assert.Equal("1234", employee.SSNLast4);
        Assert.Equal(PayType.Hourly, employee.PayType);
        Assert.Equal(25.50m, employee.HourlyRate);
    }

    [Fact]
    public async Task ValidateAsync_RejectsDuplicateEmployeeNumber()
    {
        await using var context = CreateContext("employees_import_duplicate_number");
        context.Companies.Add(CreateCompany());
        context.Employees.Add(CreateExistingEmployee());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName,EmployeeNumber,PayType,HourlyRate\nGrace,Hopper,E001,Hourly,30");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "DuplicateEmployeeNumber");
    }

    [Fact]
    public async Task ValidateAsync_RejectsFullSsnMappedAsSsnLast4()
    {
        await using var context = CreateContext("employees_import_full_ssn");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName,SSN,PayType,HourlyRate\nAda,Lovelace,123456789,Hourly,25");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var mappings = new[]
        {
            new ImportMappingInput { SourceColumn = "FirstName", TargetField = "FirstName" },
            new ImportMappingInput { SourceColumn = "LastName", TargetField = "LastName" },
            new ImportMappingInput { SourceColumn = "SSN", TargetField = "SSNLast4" },
            new ImportMappingInput { SourceColumn = "PayType", TargetField = "PayType" },
            new ImportMappingInput { SourceColumn = "HourlyRate", TargetField = "HourlyRate" }
        };

        var validation = await service.ValidateAsync(1, "employees.csv", parseResult, mappings, demoModeWarningAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "InvalidSSNLast4");
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "SensitiveColumnMapped");
    }

    [Fact]
    public async Task ValidateAsync_RequiresRateForPayType()
    {
        await using var context = CreateContext("employees_import_missing_rate");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName,EmployeeNumber,PayType\nAda,Lovelace,E002,Hourly");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "HourlyRateRequired");
    }

    [Fact]
    public async Task ValidateAsync_AllowsFirstNameAndLastNameMapping()
    {
        await using var context = CreateContext("employees_import_first_last_passes");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName,EmployeeNumber,PayType,HourlyRate\nAda,Lovelace,E010,Hourly,25");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        Assert.True(validation.CanImport);
    }

    [Fact]
    public async Task ValidateAsync_AllowsFullNameWithoutFirstNameAndLastName()
    {
        await using var context = CreateContext("employees_import_full_name_passes");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FullName,EmployeeNumber,PayType,HourlyRate\nJohn Smith,E011,Hourly,25");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        Assert.True(validation.CanImport);

        var confirmation = await service.ConfirmAsync(1, validation.Batch.ImportBatchId);
        var employee = await context.Employees.SingleAsync();
        Assert.Equal(1, confirmation.ImportedEmployeeCount);
        Assert.Equal("John", employee.FirstName);
        Assert.Equal("Smith", employee.LastName);
    }

    [Fact]
    public async Task ValidateAsync_RejectsFirstNameWithoutLastNameOrFullName()
    {
        await using var context = CreateContext("employees_import_first_only_fails");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,EmployeeNumber,PayType,HourlyRate\nAda,E012,Hourly,25");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "EmployeeNameMappingMissing");
    }

    [Fact]
    public async Task ValidateAsync_RejectsLastNameWithoutFirstNameOrFullName()
    {
        await using var context = CreateContext("employees_import_last_only_fails");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("LastName,EmployeeNumber,PayType,HourlyRate\nLovelace,E013,Hourly,25");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");

        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "EmployeeNameMappingMissing");
    }

    [Fact]
    public void ParseFullName_ParsesFirstLast()
    {
        var result = EmployeesOnlyImportService.ParseFullName("John Smith");

        Assert.False(result.NeedsReview);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.LastName);
    }

    [Fact]
    public void ParseFullName_ParsesLastCommaFirst()
    {
        var result = EmployeesOnlyImportService.ParseFullName("Smith, John");

        Assert.False(result.NeedsReview);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.LastName);
    }

    [Fact]
    public void ParseFullName_FlagsUncertainNamesForReview()
    {
        var result = EmployeesOnlyImportService.ParseFullName("John");

        Assert.True(result.NeedsReview);
    }

    [Fact]
    public async Task ConfirmAsync_BlocksWhenValidationErrorsExist()
    {
        await using var context = CreateContext("employees_import_confirm_blocked");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream("FirstName,LastName,PayType\nAda,Lovelace,Hourly");
        var parseResult = await service.ParseFileAsync(stream, "employees.csv");
        var validation = await service.ValidateAsync(
            1,
            "employees.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            demoModeWarningAcknowledged: true);

        await Assert.ThrowsAsync<ValidationException>(() => service.ConfirmAsync(1, validation.Batch.ImportBatchId));
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    private static EmployeesOnlyImportService CreateService(PayrollDbContext context)
    {
        var importService = new ImportService(context, new IImportFileParser[]
        {
            new CsvImportFileParser(),
            new TabDelimitedImportFileParser(),
            new ExcelImportFileParser()
        });

        return new EmployeesOnlyImportService(context, importService);
    }

    private static IEnumerable<ImportMappingInput> CreateMappings(IEnumerable<string> columns)
    {
        return columns.Select(column => new ImportMappingInput
        {
            SourceColumn = column,
            TargetField = column
        });
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

    private static Employee CreateExistingEmployee()
    {
        return new Employee
        {
            CompanyId = 1,
            FirstName = "Ada",
            LastName = "Lovelace",
            SSNLast4 = "1234",
            EmployeeNumber = "E001",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2026, 1, 1),
            EmploymentStatus = EmploymentStatus.Active,
            WorkerType = WorkerType.W2Employee,
            PayType = PayType.Hourly,
            HourlyRate = 25m,
            ResidenceAddress = "100 Local Street",
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
