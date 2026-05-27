using ClearPathPayroll.Configuration;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClearPathPayroll.Tests;

public class CheckCalculationPreviewServiceTests
{
    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task RecalculateCheckAsync_SavesLineItemsTotalsAndAuditLog()
    {
        await using var context = CreateContext("check_calc_recalculate_one");
        await SeedPayrollRunAsync(context, PayrollStatus.Draft);
        var service = CreateService(context, allowSandboxTaxLookup: false);

        var result = await service.RecalculateCheckAsync(100, new CheckCalculationInput
        {
            PayrollRunEmployeeId = 300,
            RegularHours = 40m,
            OvertimeHours = 2m,
            AdditionalEarnings = 100m,
            ManualDeductions = 25m,
            Reimbursements = 10m,
            Notes = "Local preview"
        }, "tester");

        var check = await context.PayrollRunEmployees
            .Include(pre => pre.EarningLines)
            .Include(pre => pre.DeductionLines)
            .Include(pre => pre.TaxLines)
            .Include(pre => pre.NetPayLines)
            .SingleAsync(pre => pre.PayrollRunEmployeeId == 300);
        var run = await context.PayrollRuns.SingleAsync(run => run.PayrollRunId == 100);

        Assert.True(result.GrossPay > 0);
        Assert.Contains(check.EarningLines, line => line.Description == "Regular Pay");
        Assert.Contains(check.EarningLines, line => line.Description == "Overtime Pay Placeholder");
        Assert.Contains(check.EarningLines, line => line.Description == "Additional Earnings");
        Assert.Contains(check.EarningLines, line => line.Description == "Reimbursements");
        Assert.Contains(check.DeductionLines, line => line.Description == "Manual Deductions");
        Assert.Single(check.TaxLines);
        Assert.Single(check.NetPayLines);
        Assert.Equal(PayrollStatus.Calculated, run.Status);
        Assert.Contains(context.AuditLogEntries, entry => entry.EventType == "CheckRecalculated");
    }

    [Fact]
    public async Task RecalculateAllAsync_BlocksApprovedPayrollRun()
    {
        await using var context = CreateContext("check_calc_blocks_approved");
        await SeedPayrollRunAsync(context, PayrollStatus.Approved);
        var service = CreateService(context, allowSandboxTaxLookup: false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RecalculateAllAsync(100, new[]
        {
            new CheckCalculationInput { PayrollRunEmployeeId = 300, RegularHours = 40m }
        }));
    }

    [Fact]
    public async Task RecalculateCheckAsync_UsesFakeTaxWhenSandboxLookupDisabled()
    {
        await using var context = CreateContext("check_calc_fake_tax");
        await SeedPayrollRunAsync(context, PayrollStatus.Draft);
        var countingTaxService = new CountingTaxCalculationService();
        var service = CreateService(context, allowSandboxTaxLookup: false, countingTaxService);

        var result = await service.RecalculateCheckAsync(100, new CheckCalculationInput
        {
            PayrollRunEmployeeId = 300,
            RegularHours = 40m
        });

        Assert.False(result.UsedConfiguredSandboxTaxService);
        Assert.Equal(0, countingTaxService.LegacyCallCount);
        Assert.Contains(result.Warnings, warning => warning.Contains("Fake tax calculation service"));
    }

    [Fact]
    public async Task RecalculateCheckAsync_UsesConfiguredServiceWhenSandboxLookupEnabled()
    {
        await using var context = CreateContext("check_calc_configured_tax");
        await SeedPayrollRunAsync(context, PayrollStatus.Draft);
        var countingTaxService = new CountingTaxCalculationService();
        var service = CreateService(context, allowSandboxTaxLookup: true, countingTaxService);

        var result = await service.RecalculateCheckAsync(100, new CheckCalculationInput
        {
            PayrollRunEmployeeId = 300,
            RegularHours = 40m
        });

        Assert.True(result.UsedConfiguredSandboxTaxService);
        Assert.Equal(1, countingTaxService.LegacyCallCount);
    }

    private static CheckCalculationPreviewService CreateService(
        PayrollDbContext context,
        bool allowSandboxTaxLookup,
        ITaxCalculationService? taxService = null)
    {
        return new CheckCalculationPreviewService(
            context,
            taxService ?? new CountingTaxCalculationService(),
            Options.Create(new LimitedLiabilityModeOptions { Enabled = true, AllowExternalTaxApiLookup = allowSandboxTaxLookup }),
            Options.Create(new TaxApiOptions { SandboxMode = true, BaseUrl = "https://sandbox.invalid", ApiKey = "sandbox" }));
    }

    private static async Task SeedPayrollRunAsync(PayrollDbContext context, PayrollStatus status)
    {
        context.Companies.Add(new Company
        {
            CompanyId = 1,
            LegalName = "Demo Company LLC",
            FEIN = "00-0000000",
            PrimaryAddress = "100 Local Way",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102"
        });
        context.PaySchedules.Add(new PaySchedule
        {
            PayScheduleId = 10,
            CompanyId = 1,
            Name = "Biweekly",
            Frequency = PayFrequency.Biweekly,
            NextPayDate = new DateTime(2026, 6, 5),
            NextPeriodStartDate = new DateTime(2026, 5, 16),
            NextPeriodEndDate = new DateTime(2026, 5, 31)
        });
        context.Employees.Add(new Employee
        {
            EmployeeId = 200,
            CompanyId = 1,
            FirstName = "Avery",
            LastName = "Local",
            SSNLast4 = "1234",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2026, 1, 1),
            EmploymentStatus = EmploymentStatus.Active,
            WorkerType = WorkerType.W2Employee,
            PayType = PayType.Hourly,
            HourlyRate = 25m,
            Address1 = "100 Local Way",
            ResidenceAddress = "100 Local Way",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102"
        });
        context.PayrollRuns.Add(new PayrollRun
        {
            PayrollRunId = 100,
            CompanyId = 1,
            PayScheduleId = 10,
            PayPeriodStart = new DateTime(2026, 5, 16),
            PayPeriodEnd = new DateTime(2026, 5, 31),
            PayDate = new DateTime(2026, 6, 5),
            Status = status
        });
        context.PayrollRunEmployees.Add(new PayrollRunEmployee
        {
            PayrollRunEmployeeId = 300,
            PayrollRunId = 100,
            EmployeeId = 200
        });

        await context.SaveChangesAsync();
    }

    private sealed class CountingTaxCalculationService : ITaxCalculationService
    {
        public int LegacyCallCount { get; private set; }

        public Task<TaxCalculationResult> CalculateTaxesAsync(decimal grossPay, string state, string employeeState = "OK")
        {
            LegacyCallCount++;
            return Task.FromResult(new TaxCalculationResult
            {
                FederalIncomeTax = 10m,
                StateIncomeTax = 5m,
                SocialSecurityTax = 6m,
                MedicareTax = 2m,
                TotalEmployeeTaxes = 23m,
                EmployerSocialSecurityTax = 6m,
                EmployerMedicareTax = 2m,
                TotalEmployerTaxes = 8m
            });
        }

        public Task<TaxCalculationResponse> CalculateTaxesAsync(TaxCalculationRequest request)
        {
            return Task.FromResult(new TaxCalculationResponse { IsSuccessful = true });
        }
    }
}
