using System;
using System.Linq;
using System.Threading.Tasks;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class DirectDepositSubmissionServiceTests
{
    private PayrollDbContext CreateInMemoryContext(string name)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task SubmitPayrollRun_Succeeds()
    {
        var ctx = CreateInMemoryContext("dd_success");

        var company = new Company { CompanyId = 1, LegalName = "Test Co", FEIN = "12-3456789" };
        ctx.Companies.Add(company);

        var funding = new CompanyFundingAccount { CompanyFundingAccountId = 10, CompanyId = 1, RoutingNumberToken = "r_tok", AccountNumberToken = "a_tok", Last4 = "0001", IsActive = true };
        ctx.CompanyFundingAccounts.Add(funding);

        var employee = new Employee { EmployeeId = 1, CompanyId = 1, FirstName = "John", LastName = "Doe", SSNLast4 = "1234", DateOfBirth = DateTime.UtcNow.AddYears(-30), HireDate = DateTime.UtcNow.AddYears(-1), ResidenceAddress = "1 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly };
        ctx.Employees.Add(employee);

        var payrollRun = new PayrollRun { PayrollRunId = 500, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Approved, TotalGrossPay = 1000 };
        ctx.PayrollRuns.Add(payrollRun);

        var pre = new PayrollRunEmployee { PayrollRunEmployeeId = 600, PayrollRunId = 500, EmployeeId = 1, GrossPay = 1000, TotalDeductions = 0, TotalTaxes = 0, NetPay = 1000 };
        pre.NetPayLines.Add(new NetPayLine { NetPayLineId = 700, Amount = 1000, PaymentMethod = "DirectDeposit" });
        ctx.PayrollRunEmployees.Add(pre);

        var bank = new EmployeeBankAccount { EmployeeBankAccountId = 800, EmployeeId = 1, BankName = "Bank", AccountNumberToken = "tok", RoutingNumberToken = "rtok", Last4 = "1111", VerificationStatus = VerificationStatus.Verified, IsActive = true };
        ctx.EmployeeBankAccounts.Add(bank);

        await ctx.SaveChangesAsync();

        var ach = new FakeAchPaymentService();
        var payrollService = new PayrollService(ctx);
        var submissionService = new DirectDepositSubmissionService(ctx, ach, payrollService);

        var batch = await submissionService.SubmitPayrollRunDirectDepositAsync(500, 10, "tester", sandboxMode: true);

        Assert.Equal(DirectDepositBatchStatus.Submitted, batch.Status);
        Assert.False(string.IsNullOrEmpty(batch.ExternalBatchReference));

        var dbBatch = await ctx.DirectDepositBatches.Include(b => b.Items).FirstOrDefaultAsync(b => b.BatchId == batch.BatchId);
        Assert.NotNull(dbBatch);
        Assert.True(dbBatch.Items.Count > 0);
    }

    [Fact]
    public async Task SubmitPayrollRun_Fails_WhenDuplicate()
    {
        var ctx = CreateInMemoryContext("dd_duplicate");

        var payrollRun = new PayrollRun { PayrollRunId = 501, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Approved, TotalGrossPay = 100 };
        ctx.PayrollRuns.Add(payrollRun);

        var batch = new DirectDepositBatch { BatchId = 900, PayrollRunId = 501, CompanyId = 1, CompanyFundingAccountId = 10, PayDate = DateTime.UtcNow, Status = DirectDepositBatchStatus.Submitted, TotalAmount = 100 };
        ctx.DirectDepositBatches.Add(batch);

        await ctx.SaveChangesAsync();

        var ach = new FakeAchPaymentService();
        var payrollService = new PayrollService(ctx);
        var submissionService = new DirectDepositSubmissionService(ctx, ach, payrollService);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await submissionService.SubmitPayrollRunDirectDepositAsync(501, 10, "tester"));
    }

    [Fact]
    public async Task SubmitPayrollRun_Handles_ApiFailure()
    {
        var ctx = CreateInMemoryContext("dd_api_fail");

        var company = new Company { CompanyId = 2, LegalName = "Test Co 2", FEIN = "98-7654321" };
        ctx.Companies.Add(company);

        var funding = new CompanyFundingAccount { CompanyFundingAccountId = 11, CompanyId = 2, RoutingNumberToken = "r_tok", AccountNumberToken = "a_tok", Last4 = "0002", IsActive = true };
        ctx.CompanyFundingAccounts.Add(funding);

        var employee = new Employee { EmployeeId = 3, CompanyId = 2, FirstName = "Jane", LastName = "Smith", SSNLast4 = "5678", DateOfBirth = DateTime.UtcNow.AddYears(-25), HireDate = DateTime.UtcNow.AddYears(-2), ResidenceAddress = "2 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly };
        ctx.Employees.Add(employee);

        var payrollRun = new PayrollRun { PayrollRunId = 502, CompanyId = 2, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Approved, TotalGrossPay = 500 };
        ctx.PayrollRuns.Add(payrollRun);

        var pre = new PayrollRunEmployee { PayrollRunEmployeeId = 601, PayrollRunId = 502, EmployeeId = 3, GrossPay = 500, TotalDeductions = 0, TotalTaxes = 0, NetPay = 500 };
        pre.NetPayLines.Add(new NetPayLine { NetPayLineId = 701, Amount = 500, PaymentMethod = "DirectDeposit" });
        ctx.PayrollRunEmployees.Add(pre);

        var bank = new EmployeeBankAccount { EmployeeBankAccountId = 801, EmployeeId = 3, BankName = "Bank", AccountNumberToken = "tok", RoutingNumberToken = "rtok", Last4 = "2222", VerificationStatus = VerificationStatus.Verified, IsActive = true };
        ctx.EmployeeBankAccounts.Add(bank);

        await ctx.SaveChangesAsync();

        // Create a fake ACH service that throws
        var ach = new ThrowingAchService();
        var payrollService = new PayrollService(ctx);
        var submissionService = new DirectDepositSubmissionService(ctx, ach, payrollService);

        var batch = await submissionService.SubmitPayrollRunDirectDepositAsync(502, 11, "tester");

        Assert.Equal(DirectDepositBatchStatus.Failed, batch.Status);
        Assert.False(string.IsNullOrEmpty(batch.ErrorMessage));

        var audit = ctx.AuditLogEntries.FirstOrDefault(a => a.EventType == "DirectDepositSubmissionFailed" && a.PayrollRunId == 502);
        Assert.NotNull(audit);
    }
}

// Helper ACH service that throws
public class ThrowingAchService : IAchPaymentService
{
    public Task<AchBatchResponse> SubmitBatchAsync(AchBatchRequest request)
    {
        throw new Exception("Simulated ACH provider failure");
    }

    public Task<AchBatchResponse> GetBatchStatusAsync(string batchReference)
    {
        throw new NotImplementedException();
    }

    public Task<AchBatchResponse> CancelBatchAsync(string batchReference)
    {
        throw new NotImplementedException();
    }
}
