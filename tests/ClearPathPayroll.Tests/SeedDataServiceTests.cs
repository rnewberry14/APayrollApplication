using ClearPathPayroll.Configuration;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;

namespace ClearPathPayroll.Tests;

public class SeedDataServiceTests : IDisposable
{
    private readonly PayrollDbContext _context;
    private readonly SeedDataService _service;

    public SeedDataServiceTests()
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName: $"SeedData_{Guid.NewGuid():N}")
            .Options;

        _context = new PayrollDbContext(options);

        var hostEnvironment = new TestHostEnvironment
        {
            EnvironmentName = Environments.Development,
            ContentRootPath = Directory.GetCurrentDirectory(),
            ContentRootFileProvider = null!
        };

        var prototypeOptions = Options.Create(new PrototypeModeOptions
        {
            Enabled = true,
            LocalDbDatabaseName = "ClearPathPayroll.LocalPrototype.Test"
        });

        _service = new SeedDataService(_context, hostEnvironment, prototypeOptions);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SeedDemoDataAsync_CreatesDemoCompanyAndPayrollRun()
    {
        var result = await _service.SeedDemoDataAsync();

        Assert.False(result.AlreadyExists);
        Assert.Equal("Demo Company (Local Prototype)", result.CompanyName);
        Assert.Equal(3, result.EmployeeCount);
        Assert.True(result.CompanyId > 0);
        Assert.True(result.PayrollRunId > 0);

        var company = await _context.Companies.FindAsync(result.CompanyId);
        Assert.NotNull(company);

        var payrollRun = await _context.PayrollRuns.FindAsync(result.PayrollRunId);
        Assert.NotNull(payrollRun);
        Assert.Equal(PayrollStatus.Draft, payrollRun!.Status);
    }

    [Fact]
    public async Task SeedDemoDataAsync_IsIdempotent_WhenCalledAgain_ReturnsAlreadyExists()
    {
        var first = await _service.SeedDemoDataAsync();
        var second = await _service.SeedDemoDataAsync();

        Assert.False(first.AlreadyExists);
        Assert.True(second.AlreadyExists);
        Assert.Equal(first.CompanyId, second.CompanyId);
        Assert.Equal(first.PayrollRunId, second.PayrollRunId);
    }

    [Fact]
    public async Task SeedDemoDataAsync_ThrowsWhenNotAllowedInProduction()
    {
        var hostEnvironment = new TestHostEnvironment
        {
            EnvironmentName = Environments.Production,
            ContentRootPath = Directory.GetCurrentDirectory(),
            ContentRootFileProvider = null!
        };

        var prototypeOptions = Options.Create(new PrototypeModeOptions { Enabled = false, LocalDbDatabaseName = "ClearPathPayroll.LocalPrototype.Test" });
        var service = new SeedDataService(_context, hostEnvironment, prototypeOptions);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SeedDemoDataAsync());
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string? WebRootPath { get; set; }
        public IFileProvider? WebRootFileProvider { get; set; }
    }
}
