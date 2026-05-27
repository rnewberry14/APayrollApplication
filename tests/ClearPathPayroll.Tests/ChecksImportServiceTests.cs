using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace ClearPathPayroll.Tests;

public class ChecksImportServiceTests
{
    [Fact]
    public async Task ConfirmAsync_CreatesDraftAfterTheFactPayrollRun()
    {
        await using var context = CreateContext("checks_import_confirm");
        context.Companies.Add(CreateCompany());
        context.Employees.Add(CreateEmployee());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv());
        var parseResult = await service.ParseFileAsync(stream, "checks.csv");

        var validation = await service.ValidateAsync(
            ChecksImportMode.ChecksOnly,
            "checks.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            reviewAcknowledged: true);

        Assert.True(validation.CanImport);

        var confirmation = await service.ConfirmAsync(ChecksImportMode.ChecksOnly, validation.Batch.ImportBatchId);

        Assert.Equal(1, confirmation.CheckCount);
        var run = await context.PayrollRuns
            .Include(payrollRun => payrollRun.PayrollRunEmployees)
                .ThenInclude(employee => employee.EarningLines)
            .Include(payrollRun => payrollRun.PayrollRunEmployees)
                .ThenInclude(employee => employee.TaxLines)
            .Include(payrollRun => payrollRun.PayrollRunEmployees)
                .ThenInclude(employee => employee.DeductionLines)
            .Include(payrollRun => payrollRun.PayrollRunEmployees)
                .ThenInclude(employee => employee.NetPayLines)
            .SingleAsync();

        Assert.Equal(PayrollStatus.Draft, run.Status);
        Assert.Equal("After-the-fact payroll", run.PayrollMode);
        Assert.Equal(1000m, run.TotalGrossPay);
        Assert.Equal(751.75m, run.TotalNetPay);
        Assert.Single(run.PayrollRunEmployees);
        Assert.NotEmpty(run.PayrollRunEmployees.Single().TaxLines);
        Assert.Single(run.PayrollRunEmployees.Single().NetPayLines);
    }

    [Fact]
    public async Task ValidateAsync_FlagsNetPayMismatch()
    {
        await using var context = CreateContext("checks_import_mismatch");
        context.Companies.Add(CreateCompany());
        context.Employees.Add(CreateEmployee());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv().Replace("751.75", "800.00", StringComparison.Ordinal));
        var parseResult = await service.ParseFileAsync(stream, "checks.csv");

        var validation = await service.ValidateAsync(
            ChecksImportMode.ChecksOnly,
            "checks.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            reviewAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "NetPayMismatch");
    }

    [Fact]
    public async Task ValidateAsync_ChecksOnlyRequiresExistingEmployee()
    {
        await using var context = CreateContext("checks_import_missing_employee");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv());
        var parseResult = await service.ParseFileAsync(stream, "checks.csv");

        var validation = await service.ValidateAsync(
            ChecksImportMode.ChecksOnly,
            "checks.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            reviewAcknowledged: true);

        Assert.False(validation.CanImport);
        Assert.Contains(validation.Batch.Errors, error => error.ErrorCode == "EmployeeNotFound");
    }

    [Fact]
    public async Task ConfirmAsync_EmployeesAndChecksCreatesPlaceholderEmployee()
    {
        await using var context = CreateContext("checks_import_employee_placeholder");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv());
        var parseResult = await service.ParseFileAsync(stream, "checks.csv");

        var validation = await service.ValidateAsync(
            ChecksImportMode.EmployeesAndChecks,
            "checks.csv",
            parseResult,
            CreateMappings(parseResult.Columns),
            reviewAcknowledged: true);

        Assert.True(validation.CanImport);

        await service.ConfirmAsync(ChecksImportMode.EmployeesAndChecks, validation.Batch.ImportBatchId);

        var employee = await context.Employees.SingleAsync();
        Assert.Equal("E001", employee.EmployeeNumber);
        Assert.Equal("0000", employee.SSNLast4);
    }

    [Fact]
    public async Task ConfirmAsync_BlocksWhenValidationFails()
    {
        await using var context = CreateContext("checks_import_confirm_blocked");
        context.Companies.Add(CreateCompany());
        context.Employees.Add(CreateEmployee());
        await context.SaveChangesAsync();
        var service = CreateService(context);
        await using var stream = ToStream(ValidCsv().Replace("751.75", "800.00", StringComparison.Ordinal));
        var parseResult = await service.ParseFileAsync(stream, "checks.csv");
        var validation = await service.ValidateAsync(ChecksImportMode.ChecksOnly, "checks.csv", parseResult, CreateMappings(parseResult.Columns), true);

        await Assert.ThrowsAsync<ValidationException>(() => service.ConfirmAsync(ChecksImportMode.ChecksOnly, validation.Batch.ImportBatchId));
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    private static ChecksImportService CreateService(PayrollDbContext context)
    {
        var importService = new ImportService(context, new IImportFileParser[]
        {
            new CsvImportFileParser(),
            new TabDelimitedImportFileParser(),
            new ExcelImportFileParser()
        });

        return new ChecksImportService(context, importService);
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
        return "CompanyIdentifier,EmployeeIdentifier,PayDate,PayPeriodStart,PayPeriodEnd,GrossPay,RegularHours,OvertimeHours,EmployeeFederalTax,EmployeeStateTax,EmployeeLocalTax,SocialSecurityTax,MedicareTax,Deductions,NetPay,PaymentMethod,CheckNumber,DirectDepositLast4\n" +
            "Demo Company LLC,E001,2026-05-15,2026-05-01,2026-05-14,1000.00,40,0,120.00,40.00,0,62.00,14.25,12.00,751.75,Check,1001,";
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

    private static Employee CreateEmployee()
    {
        return new Employee
        {
            CompanyId = 1,
            EmployeeNumber = "E001",
            FirstName = "Ada",
            LastName = "Lovelace",
            SSNLast4 = "1234",
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
