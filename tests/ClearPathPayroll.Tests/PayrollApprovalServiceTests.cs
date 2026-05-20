using System;
using System.Linq;
using System.Threading.Tasks;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollApprovalServiceTests
{
    private PayrollDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task ApprovePayrollRun_Succeeds_WhenAllValid()
    {
        var ctx = CreateInMemoryContext("approve_success");

        var employee = new Employee { EmployeeId = 1, CompanyId = 1, FirstName = "John", LastName = "Doe", SSNLast4 = "1234", DateOfBirth = DateTime.UtcNow.AddYears(-30), HireDate = DateTime.UtcNow.AddYears(-1), ResidenceAddress = "1 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly };
        ctx.Employees.Add(employee);

        var payrollRun = new PayrollRun { PayrollRunId = 100, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Calculated, TotalGrossPay = 1000 };
        ctx.PayrollRuns.Add(payrollRun);

        var pre = new PayrollRunEmployee { PayrollRunEmployeeId = 200, PayrollRunId = 100, EmployeeId = 1, GrossPay = 1000, TotalDeductions = 0, TotalTaxes = 0, NetPay = 1000 };
        pre.NetPayLines.Add(new NetPayLine { NetPayLineId = 300, Amount = 1000, PaymentMethod = "DirectDeposit" });
        ctx.PayrollRunEmployees.Add(pre);

        var bank = new EmployeeBankAccount { EmployeeBankAccountId = 400, EmployeeId = 1, BankName = "Bank", AccountNumberToken = "tok", RoutingNumberToken = "tok", Last4 = "1111", VerificationStatus = VerificationStatus.Verified, IsActive = true };
        ctx.EmployeeBankAccounts.Add(bank);

        await ctx.SaveChangesAsync();

        var payrollService = new PayrollService(ctx);
        var approvalService = new PayrollApprovalService(ctx, payrollService);

        var result = await approvalService.ApprovePayrollRunAsync(100, "tester");

        Assert.True(result);

        var updated = await ctx.PayrollRuns.FindAsync(100);
        Assert.Equal(PayrollStatus.Approved, updated!.Status);
        Assert.Equal("tester", updated.ApprovedByUserId);
        Assert.NotNull(updated.ApprovedAt);

        var audit = ctx.AuditLogEntries.FirstOrDefault(a => a.EventType == "PayrollRunApproved" && a.PayrollRunId == 100);
        Assert.NotNull(audit);
    }

    [Fact]
    public async Task ApprovePayrollRun_Fails_WhenNotCalculated()
    {
        var ctx = CreateInMemoryContext("approve_not_calc");

        var payrollRun = new PayrollRun { PayrollRunId = 101, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Draft, TotalGrossPay = 1000 };
        ctx.PayrollRuns.Add(payrollRun);
        await ctx.SaveChangesAsync();

        var payrollService = new PayrollService(ctx);
        var approvalService = new PayrollApprovalService(ctx, payrollService);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await approvalService.ApprovePayrollRunAsync(101, "tester"));
    }

    [Fact]
    public async Task ApprovePayrollRun_Fails_WhenBankNotVerified()
    {
        var ctx = CreateInMemoryContext("approve_bank_fail");

        var employee = new Employee { EmployeeId = 2, CompanyId = 1, FirstName = "Jane", LastName = "Smith", SSNLast4 = "5678", DateOfBirth = DateTime.UtcNow.AddYears(-25), HireDate = DateTime.UtcNow.AddYears(-2), ResidenceAddress = "2 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly };
        ctx.Employees.Add(employee);

        var payrollRun = new PayrollRun { PayrollRunId = 102, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Calculated, TotalGrossPay = 500 };
        ctx.PayrollRuns.Add(payrollRun);

        var pre = new PayrollRunEmployee { PayrollRunEmployeeId = 201, PayrollRunId = 102, EmployeeId = 2, GrossPay = 500, TotalDeductions = 0, TotalTaxes = 0, NetPay = 500 };
        pre.NetPayLines.Add(new NetPayLine { NetPayLineId = 301, Amount = 500, PaymentMethod = "DirectDeposit" });
        ctx.PayrollRunEmployees.Add(pre);

        // Add bank record but not verified
        var bank = new EmployeeBankAccount { EmployeeBankAccountId = 401, EmployeeId = 2, BankName = "Bank", AccountNumberToken = "tok", RoutingNumberToken = "tok", Last4 = "2222", VerificationStatus = VerificationStatus.Pending, IsActive = true };
        ctx.EmployeeBankAccounts.Add(bank);

        await ctx.SaveChangesAsync();

        var payrollService = new PayrollService(ctx);
        var approvalService = new PayrollApprovalService(ctx, payrollService);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await approvalService.ApprovePayrollRunAsync(102, "tester"));
        Assert.Contains("missing verified bank accounts", ex.Message);
    }
}
