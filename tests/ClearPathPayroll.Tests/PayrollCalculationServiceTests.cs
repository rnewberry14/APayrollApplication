using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollCalculationServiceTests : IDisposable
{
    private readonly PayrollDbContext _context;
    private readonly PayrollCalculationService _service;

    public PayrollCalculationServiceTests()
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName: $"PayrollCalc_{Guid.NewGuid():N}")
            .Options;

        _context = new PayrollDbContext(options);
        _service = new PayrollCalculationService(
            _context,
            new GrossPayCalculationService(),
            new DeductionCalculationService(),
            new FakeTaxCalculationService());
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private static Employee CreateHourlyEmployee(decimal hourlyRate)
        => new()
        {
            CompanyId = 1,
            FirstName = "Hourly",
            LastName = "Worker",
            SSNLast4 = "1234",
            Email = "hourly@example.com",
            PayType = PayType.Hourly,
            HourlyRate = hourlyRate,
            State = "OK",
            EmploymentStatus = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-1),
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            ResidenceAddress = "100 Market St",
            City = "Oklahoma City",
            ZipCode = "73102"
        };

    private static Employee CreateSalaryEmployee(decimal annualSalary)
        => new()
        {
            CompanyId = 1,
            FirstName = "Salary",
            LastName = "Worker",
            SSNLast4 = "5678",
            Email = "salary@example.com",
            PayType = PayType.Salary,
            AnnualSalary = annualSalary,
            State = "OK",
            EmploymentStatus = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-1),
            DateOfBirth = DateTime.UtcNow.AddYears(-35),
            ResidenceAddress = "200 Main St",
            City = "Oklahoma City",
            ZipCode = "73102"
        };

    private static PaySchedule CreateBiweeklyPaySchedule()
        => new()
        {
            CompanyId = 1,
            Name = "Biweekly",
            Frequency = PayFrequency.Biweekly,
            NextPayDate = DateTime.UtcNow.AddDays(1),
            NextPeriodStartDate = DateTime.UtcNow.AddDays(1),
            NextPeriodEndDate = DateTime.UtcNow.AddDays(14),
            IsActive = true
        };

    private static PayrollRun CreatePayrollRun(int payScheduleId)
        => new()
        {
            CompanyId = 1,
            PayScheduleId = payScheduleId,
            PayPeriodStart = DateTime.UtcNow.Date,
            PayPeriodEnd = DateTime.UtcNow.Date.AddDays(13),
            PayDate = DateTime.UtcNow.Date.AddDays(14),
            Status = PayrollStatus.Draft,
            TotalGrossPay = 0m,
            TotalEmployeeTaxes = 0m,
            TotalEmployerTaxes = 0m,
            TotalDeductions = 0m,
            TotalNetPay = 0m
        };

    [Fact]
    public async Task HourlyEmployee_CalculatesGrossPay()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateHourlyEmployee(25m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var result = await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, new List<DeductionInput>(), 40m, "tester");

        Assert.Equal(1000m, result.GrossPay);
        Assert.True(result.TotalEmployeeTaxes > 0);
        Assert.True(result.NetPay > 0);
        Assert.True(result.NetPay < result.GrossPay);
    }

    [Fact]
    public async Task SalaryEmployee_CalculatesGrossPay()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var result = await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, new List<DeductionInput>(), null, "tester");

        Assert.Equal(2000m, result.GrossPay);
        Assert.True(result.TotalEmployeeTaxes > 0);
        Assert.True(result.NetPay > 0);
    }

    [Fact]
    public async Task PreTaxDeduction_ReducesTaxableWages()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var deductions = new List<DeductionInput>
        {
            new() { Description = "401(k)", Type = DeductionType.PreTaxFixedAmount, Amount = 200m }
        };

        var result = await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, deductions, null, "tester");

        Assert.Equal(200m, result.PreTaxDeductions);
        Assert.Equal(1800m, result.TaxableWages);
        Assert.Contains(result.DeductionLines, x => x.Description == "401(k)" && x.Type == "Pre-Tax");
    }

    [Fact]
    public async Task PostTaxDeduction_ReducesNetPay()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var deductions = new List<DeductionInput>
        {
            new() { Description = "Health Insurance", Type = DeductionType.PostTaxFixedAmount, Amount = 150m }
        };

        var result = await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, deductions, null, "tester");

        var expectedNetPay = Math.Round(result.GrossPay - result.TotalEmployeeTaxes - 150m, 2);
        Assert.Equal(expectedNetPay, result.NetPay);
        Assert.Equal(150m, result.PostTaxDeductions);
        Assert.Contains(result.DeductionLines, x => x.Description == "Health Insurance" && x.Type == "Post-Tax");
    }

    [Fact]
    public async Task NegativeNetPay_Prevention_CapsPostTaxDeductions()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateHourlyEmployee(10m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var deductions = new List<DeductionInput>
        {
            new() { Description = "Large Post-Tax", Type = DeductionType.PostTaxFixedAmount, Amount = 2000m }
        };

        var result = await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, deductions, 20m, "tester");

        Assert.True(result.NetPay >= 0);
        Assert.True(result.PostTaxDeductions < 2000m);
    }

    [Fact]
    public async Task PayrollRunTotals_AreUpdatedAfterSave()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee1 = CreateSalaryEmployee(52000m);
        employee1.CompanyId = company.CompanyId;
        var employee2 = CreateSalaryEmployee(52000m);
        employee2.FirstName = "Second";
        employee2.CompanyId = company.CompanyId;
        _context.Employees.AddRange(employee1, employee2);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee1.EmployeeId, new List<DeductionInput>(), null, "tester");
        await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee2.EmployeeId, new List<DeductionInput>(), null, "tester");

        var refreshedPayrollRun = await _context.PayrollRuns.FirstAsync(pr => pr.PayrollRunId == payrollRun.PayrollRunId);
        Assert.Equal(4000m, refreshedPayrollRun.TotalGrossPay);
        Assert.True(refreshedPayrollRun.TotalEmployeeTaxes > 0);
        Assert.True(refreshedPayrollRun.TotalNetPay > 0);
        Assert.True(refreshedPayrollRun.TotalEmployerTaxes > 0);
    }

    [Fact]
    public async Task AuditLog_IsCreatedWhenPayrollIsCalculated()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, new List<DeductionInput>(), null, "tester");

        var auditEntries = await _context.AuditLogEntries
            .Where(a => a.PayrollRunId == payrollRun.PayrollRunId)
            .ToListAsync();

        Assert.Equal(2, auditEntries.Count);
        Assert.Contains(auditEntries, a => a.EventType == "PayrollCalculationStarted");
        Assert.Contains(auditEntries, a => a.EventType == "PayrollCalculationCompleted");
    }

    [Fact]
    public async Task RetroactivePayroll_CanBeProcessedWithOverrideDates()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        payrollRun.PayPeriodStart = DateTime.UtcNow.Date.AddMonths(-1);
        payrollRun.PayPeriodEnd = DateTime.UtcNow.Date.AddMonths(-1).AddDays(13);
        payrollRun.PayDate = DateTime.UtcNow.Date.AddMonths(-1).AddDays(14);
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var result = await _service.CalculateAndSavePayrollForEmployeeAsync(
            payrollRun.PayrollRunId,
            employee.EmployeeId,
            new List<DeductionInput>(),
            null,
            "tester",
            DateTime.UtcNow.Date.AddMonths(-1),
            DateTime.UtcNow.Date.AddMonths(-1).AddDays(13));

        Assert.Equal(2000m, result.GrossPay);
        Assert.True(result.TotalEmployeeTaxes > 0);

        var auditEntries = await _context.AuditLogEntries
            .Where(a => a.PayrollRunId == payrollRun.PayrollRunId)
            .ToListAsync();

        Assert.Contains(auditEntries, a => a.EventType == "RetroactivePayrollCalculationStarted");
    }

    [Fact]
    public async Task ManualGrossPayAdjustment_IsApplied()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var bonusAdjustment = 500m;
        var result = await _service.CalculateAndSavePayrollForEmployeeAsync(
            payrollRun.PayrollRunId,
            employee.EmployeeId,
            new List<DeductionInput>(),
            null,
            "tester",
            null,
            null,
            bonusAdjustment);

        Assert.Equal(2500m, result.GrossPay); // 2000 + 500 bonus
        Assert.Contains(result.EarningLines, x => x.Description == "Manual Adjustment" && x.Amount == bonusAdjustment);
    }

    [Fact]
    public async Task FullPayrollRun_ProcessesAllEmployees()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee1 = CreateSalaryEmployee(52000m);
        employee1.CompanyId = company.CompanyId;
        var employee2 = CreateSalaryEmployee(52000m);
        employee2.FirstName = "Second";
        employee2.CompanyId = company.CompanyId;
        _context.Employees.AddRange(employee1, employee2);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var results = await _service.ProcessFullPayrollRunAsync(payrollRun.PayrollRunId, null, null, null, "tester");

        Assert.Equal(2, results.Count);
        Assert.True(results.All(r => r.Value.GrossPay == 2000m));

        var auditEntries = await _context.AuditLogEntries
            .Where(a => a.PayrollRunId == payrollRun.PayrollRunId)
            .ToListAsync();

        Assert.Contains(auditEntries, a => a.EventType == "FullPayrollProcessingStarted");
        Assert.Contains(auditEntries, a => a.EventType == "FullPayrollProcessingCompleted");
    }

    [Fact]
    public async Task PayrollRecalculation_ClearsAndRecalculates()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        // First calculation
        await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, new List<DeductionInput>(), null, "tester");

        var firstPayrollRunEmployee = await _context.PayrollRunEmployees
            .Include(pre => pre.DeductionLines)
            .FirstAsync(pre => pre.EmployeeId == employee.EmployeeId);

        var deductions = new List<DeductionInput>
        {
            new() { Description = "401(k)", Type = DeductionType.PreTaxFixedAmount, Amount = 300m }
        };

        // Recalculate with different deductions
        var result = await _service.RecalculatePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, deductions, null, "tester");

        Assert.Equal(300m, result.PreTaxDeductions);
        Assert.Equal(1700m, result.TaxableWages);

        var auditEntries = await _context.AuditLogEntries
            .Where(a => a.PayrollRunId == payrollRun.PayrollRunId)
            .ToListAsync();

        Assert.Contains(auditEntries, a => a.EventType == "PayrollRecalculationStarted");
    }

    [Fact]
    public async Task VoidPayrollRun_ClearsAllCalculations()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee = CreateSalaryEmployee(52000m);
        employee.CompanyId = company.CompanyId;
        _context.Employees.Add(employee);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee.EmployeeId, new List<DeductionInput>(), null, "tester");

        var beforeVoid = await _context.PayrollRuns.FirstAsync(pr => pr.PayrollRunId == payrollRun.PayrollRunId);
        Assert.True(beforeVoid.TotalGrossPay > 0);

        await _service.VoidPayrollRunAsync(payrollRun.PayrollRunId, "Testing void functionality", "tester");

        var afterVoid = await _context.PayrollRuns.FirstAsync(pr => pr.PayrollRunId == payrollRun.PayrollRunId);
        Assert.Equal(PayrollStatus.Voided, afterVoid.Status);
        Assert.Equal(0m, afterVoid.TotalGrossPay);
        Assert.Equal(0m, afterVoid.TotalNetPay);
    }

    [Fact]
    public async Task PayrollRunSummary_ShowsAccurateStatus()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var employee1 = CreateSalaryEmployee(52000m);
        employee1.CompanyId = company.CompanyId;
        var employee2 = CreateSalaryEmployee(52000m);
        employee2.FirstName = "Second";
        employee2.CompanyId = company.CompanyId;
        _context.Employees.AddRange(employee1, employee2);

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        // Process only one employee
        await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee1.EmployeeId, new List<DeductionInput>(), null, "tester");

        var summary = await _service.GetPayrollRunSummaryAsync(payrollRun.PayrollRunId);

        Assert.Equal(payrollRun.PayrollRunId, summary.PayrollRunId);
        Assert.Equal("Test Company", summary.CompanyName);
        Assert.Equal("Draft", summary.Status);
        Assert.Equal(1, summary.ProcessedEmployeeCount);
        Assert.Equal(1, summary.PendingEmployeeCount);
        Assert.Equal(2, summary.TotalActiveEmployees);
        Assert.False(summary.IsComplete);

        // Process second employee
        await _service.CalculateAndSavePayrollForEmployeeAsync(payrollRun.PayrollRunId, employee2.EmployeeId, new List<DeductionInput>(), null, "tester");

        var completeSummary = await _service.GetPayrollRunSummaryAsync(payrollRun.PayrollRunId);

        Assert.Equal(2, completeSummary.ProcessedEmployeeCount);
        Assert.Equal(0, completeSummary.PendingEmployeeCount);
        Assert.True(completeSummary.IsComplete);
        Assert.True(completeSummary.TotalGrossPay > 0);
    }

    [Fact]
    public async Task CanRecalculateCheck_ReturnsTrueForDraftAndCalculated()
    {
        var company = new Company
        {
            LegalName = "Test Company",
            FEIN = "12-3456789",
            PrimaryAddress = "1 Test Blvd",
            City = "OKC",
            State = "OK",
            ZipCode = "73101"
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var paySchedule = CreateBiweeklyPaySchedule();
        paySchedule.CompanyId = company.CompanyId;
        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = CreatePayrollRun(paySchedule.PayScheduleId);
        payrollRun.CompanyId = company.CompanyId;
        payrollRun.Status = PayrollStatus.Draft;
        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var canRecalculate = await _service.CanRecalculatePayrollRunAsync(payrollRun.PayrollRunId);
        Assert.True(canRecalculate);

        payrollRun.Status = PayrollStatus.Calculated;
        _context.SaveChanges();

        canRecalculate = await _service.CanRecalculatePayrollRunAsync(payrollRun.PayrollRunId);
        Assert.True(canRecalculate);

        payrollRun.Status = PayrollStatus.Submitted;
        _context.SaveChanges();

        canRecalculate = await _service.CanRecalculatePayrollRunAsync(payrollRun.PayrollRunId);
        Assert.False(canRecalculate);
    }
}
