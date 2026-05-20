using System;
using System.Linq;
using System.Threading.Tasks;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PayStubServiceTests
{
    private PayrollDbContext CreateInMemoryContext(string name)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task GetPayStubAsync_ReturnsPayStub_WithMaskedSsnAndDirectDepositLast4()
    {
        var ctx = CreateInMemoryContext("paystub_success");

        var company = new Company { CompanyId = 1, LegalName = "Acme Payroll", FEIN = "123456789", PrimaryAddress = "1 Main St", City = "City", State = "CA", ZipCode = "90001" };
        ctx.Companies.Add(company);

        var employee = new Employee { EmployeeId = 1, CompanyId = 1, FirstName = "Jane", LastName = "Doe", SSNLast4 = "4321", DateOfBirth = DateTime.UtcNow.AddYears(-30), HireDate = DateTime.UtcNow.AddYears(-2), ResidenceAddress = "123 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly };
        ctx.Employees.Add(employee);

        var payrollRun = new PayrollRun { PayrollRunId = 1, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Calculated, TotalGrossPay = 1500 };
        ctx.PayrollRuns.Add(payrollRun);

        var payrollRunEmployee = new PayrollRunEmployee { PayrollRunEmployeeId = 1, PayrollRunId = 1, EmployeeId = 1, GrossPay = 1500, TotalDeductions = 100, TotalTaxes = 200, NetPay = 1200 };
        payrollRunEmployee.EarningLines.Add(new EarningLine { EarningLineId = 1, Description = "Regular Pay", Amount = 1500 });
        payrollRunEmployee.DeductionLines.Add(new DeductionLine { DeductionLineId = 1, Description = "Health Insurance", Amount = 100 });
        payrollRunEmployee.TaxLines.Add(new TaxLine { TaxLineId = 1, TaxType = "Federal Income Tax", Amount = 200 });
        payrollRunEmployee.NetPayLines.Add(new NetPayLine { NetPayLineId = 1, Amount = 1200, PaymentMethod = "DirectDeposit" });
        ctx.PayrollRunEmployees.Add(payrollRunEmployee);

        var bank = new EmployeeBankAccount { EmployeeBankAccountId = 1, EmployeeId = 1, BankName = "Bank", RoutingNumberToken = "rtok", AccountNumberToken = "atok", Last4 = "1111", VerificationStatus = VerificationStatus.Verified, IsActive = true };
        ctx.EmployeeBankAccounts.Add(bank);

        await ctx.SaveChangesAsync();

        var service = new PayStubService(ctx);
        var payStub = await service.GetPayStubAsync(1, 1);

        Assert.Equal("Acme Payroll", payStub.CompanyName);
        Assert.Equal("Jane Doe", payStub.EmployeeName);
        Assert.Equal("4321", payStub.SsnLast4);
        Assert.Equal("1111", payStub.DirectDepositLast4);
        Assert.Equal(1, payStub.Earnings.Count);
        Assert.Equal(1, payStub.EmployeeTaxes.Count);
        Assert.Equal(1, payStub.Deductions.Count);
        Assert.Equal(1200m, payStub.NetPay);
        Assert.Equal("Year-to-date totals will be added in a future release.", payStub.YtdTotalsPlaceholder);
    }

    [Fact]
    public async Task GetPayStubAsync_Throws_WhenPayrollRunEmployeeMissing()
    {
        var ctx = CreateInMemoryContext("paystub_missing");
        var service = new PayStubService(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetPayStubAsync(999, 999));
    }
}
