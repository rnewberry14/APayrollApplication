using ClearPathPayroll.Configuration;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClearPathPayroll.Tests;

public sealed class DemoDataSeederTests : IDisposable
{
    private readonly PayrollDbContext _context;
    private readonly DemoDataSeeder _seeder;

    public DemoDataSeederTests()
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase($"DemoDataSeeder_{Guid.NewGuid():N}")
            .Options;

        _context = new PayrollDbContext(options);
        _seeder = CreateSeeder(Environments.Development, limitedLiabilityEnabled: true);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateDemoDataAsync_CreatesRequiredFakeDemoRecords()
    {
        var result = await _seeder.CreateDemoDataAsync();

        Assert.False(result.AlreadyExists);
        Assert.Equal("Demo Company LLC", result.CompanyName);
        Assert.Equal(3, result.EmployeeCount);
        Assert.True(result.PayScheduleId > 0);
        Assert.True(result.PayrollRunId > 0);

        var company = await _context.Companies.SingleAsync(c => c.LegalName == "Demo Company LLC");
        Assert.Equal("00-0000000", company.FEIN);
        Assert.Equal("OK", company.State);

        var employees = await _context.Employees
            .Where(e => e.CompanyId == company.CompanyId)
            .ToListAsync();

        Assert.Equal(3, employees.Count);
        Assert.Contains(employees, e => e.PayType == PayType.Hourly && e.HourlyRate == 24.50m);
        Assert.Contains(employees, e => e.PayType == PayType.Salary && e.AnnualSalary == 62400m);
        Assert.Contains(employees, e => e.LastName == "Tips" && e.PayType == PayType.Hourly);
        Assert.All(employees, employee =>
        {
            Assert.Matches(@"^\d{4}$", employee.SSNLast4);
            Assert.Null(employee.FullSSNEncryptedPlaceholder);
        });

        Assert.Equal(3, await _context.EmployeeBankAccounts.CountAsync(a => a.VerificationStatus == VerificationStatus.Verified));
        Assert.Single(await _context.CompanyFundingAccounts.Where(a => a.CompanyId == company.CompanyId && a.IsPrimary).ToListAsync());

        var payrollRun = await _context.PayrollRuns.SingleAsync(p => p.CompanyId == company.CompanyId);
        Assert.Equal(PayrollStatus.Draft, payrollRun.Status);

        var tippedEarning = await _context.EarningLines.SingleAsync(e => e.Description == "Tips Placeholder");
        Assert.Equal(180m, tippedEarning.Amount);
    }

    [Fact]
    public async Task CreateDemoDataAsync_IsIdempotent()
    {
        var first = await _seeder.CreateDemoDataAsync();
        var second = await _seeder.CreateDemoDataAsync();

        Assert.False(first.AlreadyExists);
        Assert.True(second.AlreadyExists);
        Assert.Equal(1, await _context.Companies.CountAsync(c => c.LegalName == "Demo Company LLC"));
        Assert.Equal(3, await _context.Employees.CountAsync());
    }

    [Fact]
    public async Task CreateDemoDataAsync_RepairsPartialDemoData()
    {
        _context.Companies.Add(new Company
        {
            LegalName = "Demo Company LLC",
            FEIN = "00-0000000",
            State = "OK",
            IsActive = true
        });
        await _context.SaveChangesAsync();

        var result = await _seeder.CreateDemoDataAsync();

        Assert.False(result.AlreadyExists);
        Assert.Contains("repaired", result.Message);
        Assert.Equal(3, await _context.Employees.CountAsync());
        Assert.Single(await _context.PaySchedules.ToListAsync());
        Assert.Single(await _context.PayrollRuns.ToListAsync());
        Assert.Equal(3, await _context.PayrollRunEmployees.CountAsync());
        Assert.Equal(3, await _context.EmployeeBankAccounts.CountAsync(a => a.VerificationStatus == VerificationStatus.Verified));
        Assert.Single(await _context.CompanyFundingAccounts.Where(a => a.IsPrimary && a.IsActive).ToListAsync());
    }

    [Fact]
    public async Task CreateDemoDataAsync_RepairsMissingPayrollRunEmployees()
    {
        await _seeder.CreateDemoDataAsync();

        var payrollRunEmployees = await _context.PayrollRunEmployees
            .Include(e => e.EarningLines)
            .ToListAsync();
        foreach (var payrollRunEmployee in payrollRunEmployees)
        {
            _context.EarningLines.RemoveRange(payrollRunEmployee.EarningLines);
        }

        _context.PayrollRunEmployees.RemoveRange(payrollRunEmployees);
        await _context.SaveChangesAsync();

        var result = await _seeder.CreateDemoDataAsync();

        Assert.False(result.AlreadyExists);
        Assert.Contains("repaired", result.Message);
        Assert.Equal(3, await _context.PayrollRunEmployees.CountAsync());
        Assert.True(await _context.EarningLines.AnyAsync(e => e.Description == "Tips Placeholder"));
    }

    [Fact]
    public async Task ClearDemoDataAsync_RemovesSeededDemoRecords()
    {
        await _seeder.CreateDemoDataAsync();

        var cleared = await _seeder.ClearDemoDataAsync();

        Assert.True(cleared);
        Assert.False(await _context.Companies.AnyAsync(c => c.LegalName == "Demo Company LLC"));
        Assert.Empty(await _context.Employees.ToListAsync());
        Assert.Empty(await _context.EmployeeBankAccounts.ToListAsync());
        Assert.Empty(await _context.CompanyFundingAccounts.ToListAsync());
        Assert.Empty(await _context.PayrollRuns.ToListAsync());
    }

    [Fact]
    public async Task CreateDemoDataAsync_ThrowsOutsideDevelopmentAndLocalOnlyMode()
    {
        var seeder = CreateSeeder(Environments.Production, limitedLiabilityEnabled: false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.CreateDemoDataAsync());
    }

    private DemoDataSeeder CreateSeeder(string environmentName, bool limitedLiabilityEnabled)
    {
        var environment = new TestHostEnvironment { EnvironmentName = environmentName };
        var options = Options.Create(new LimitedLiabilityModeOptions
        {
            Enabled = limitedLiabilityEnabled,
            LocalDatabaseProvider = LimitedLiabilityModeHelper.SqlServerLocalDbProvider,
            LocalDatabaseName = "ClearPathPayroll.LocalOnly.Test"
        });

        return new DemoDataSeeder(_context, environment, options);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
