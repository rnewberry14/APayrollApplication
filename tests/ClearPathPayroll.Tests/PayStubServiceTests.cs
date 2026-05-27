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

        var company = new Company { CompanyId = 1, LegalName = "Acme Payroll", FEIN = "123456789", PrimaryAddress = "1 Main St", City = "City", State = "CA", ZipCode = "90001", EmployerTaxNotes = "Local employer note" };
        ctx.Companies.Add(company);

        var employee = new Employee { EmployeeId = 1, CompanyId = 1, FirstName = "Jane", LastName = "Doe", SSNLast4 = "4321", DateOfBirth = DateTime.UtcNow.AddYears(-30), HireDate = DateTime.UtcNow.AddYears(-2), Address1 = "123 Main St", ResidenceAddress = "123 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly };
        ctx.Employees.Add(employee);

        var payrollRun = new PayrollRun { PayrollRunId = 1, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Calculated, TotalGrossPay = 1500 };
        ctx.PayrollRuns.Add(payrollRun);

        var payrollRunEmployee = new PayrollRunEmployee { PayrollRunEmployeeId = 1, PayrollRunId = 1, EmployeeId = 1, GrossPay = 1500, TotalDeductions = 100, TotalTaxes = 200, NetPay = 1200 };
        payrollRunEmployee.EarningLines.Add(new EarningLine { EarningLineId = 1, Description = "Regular Pay", Hours = 40, Rate = 37.50m, Amount = 1500 });
        payrollRunEmployee.EarningLines.Add(new EarningLine { EarningLineId = 2, Description = "Reimbursements", Amount = 25 });
        payrollRunEmployee.DeductionLines.Add(new DeductionLine { DeductionLineId = 1, Description = "Pre-Tax Health Insurance", Amount = 75 });
        payrollRunEmployee.DeductionLines.Add(new DeductionLine { DeductionLineId = 2, Description = "Manual Deductions", Amount = 25 });
        payrollRunEmployee.TaxLines.Add(new TaxLine { TaxLineId = 1, TaxType = "Federal Income Tax", Amount = 200 });
        payrollRunEmployee.NetPayLines.Add(new NetPayLine { NetPayLineId = 1, Amount = 1200, PaymentMethod = "DirectDeposit" });
        ctx.PayrollRunEmployees.Add(payrollRunEmployee);
        ctx.EmployerTaxLines.Add(new EmployerTaxLine { EmployerTaxLineId = 1, PayrollRunId = 1, EmployeeId = 1, TaxType = "Employer Medicare", Amount = 20 });

        var bank = new EmployeeBankAccount { EmployeeBankAccountId = 1, EmployeeId = 1, BankName = "Bank", RoutingNumberToken = "rtok", AccountNumberToken = "atok", Last4 = "1111", VerificationStatus = VerificationStatus.Verified, IsActive = true };
        ctx.EmployeeBankAccounts.Add(bank);

        await ctx.SaveChangesAsync();

        var service = new PayStubService(ctx);
        var payStub = await service.GetPayStubAsync(1, 1);

        Assert.Equal("Acme Payroll", payStub.CompanyName);
        Assert.Contains("1 Main St", payStub.CompanyAddress);
        Assert.Equal("Jane Doe", payStub.EmployeeName);
        Assert.Contains("123 Main St", payStub.EmployeeAddress);
        Assert.Equal("4321", payStub.SsnLast4);
        Assert.Equal("1111", payStub.DirectDepositLast4);
        Assert.Single(payStub.Earnings);
        Assert.Equal(40, payStub.Earnings[0].Hours);
        Assert.Equal(37.50m, payStub.Earnings[0].Rate);
        Assert.Single(payStub.EmployeeTaxes);
        Assert.Single(payStub.EmployerTaxes);
        Assert.Single(payStub.PreTaxDeductions);
        Assert.Single(payStub.PostTaxDeductions);
        Assert.Single(payStub.Reimbursements);
        Assert.Equal(1500m, payStub.GrossPay);
        Assert.Equal(1200m, payStub.NetPay);
        Assert.Equal("DirectDeposit", payStub.PaymentMethod);
        Assert.Equal("Local employer note", payStub.EmployerNotes);
        Assert.Contains("Review applicable pay statement requirements before distribution.", payStub.RequiredFieldsChecklistPlaceholder);
        Assert.Equal("Year-to-date placeholders are available for local tracking.", payStub.YtdTotalsPlaceholder);
    }

    [Fact]
    public async Task GetPayStubByPayrollRunEmployeeIdAsync_ReturnsCheckNumberPlaceholderForCheckPayment()
    {
        var ctx = CreateInMemoryContext("paystub_check_payment");

        ctx.Companies.Add(new Company { CompanyId = 1, LegalName = "Acme Payroll", FEIN = "123456789", PrimaryAddress = "1 Main St", City = "City", State = "CA", ZipCode = "90001" });
        ctx.Employees.Add(new Employee { EmployeeId = 2, CompanyId = 1, FirstName = "Alex", LastName = "Check", SSNLast4 = "9876", DateOfBirth = DateTime.UtcNow.AddYears(-30), HireDate = DateTime.UtcNow.AddYears(-2), Address1 = "2 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly });
        ctx.PayrollRuns.Add(new PayrollRun { PayrollRunId = 2, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow, PayDate = DateTime.UtcNow.AddDays(1), Status = PayrollStatus.Calculated });
        var payrollRunEmployee = new PayrollRunEmployee { PayrollRunEmployeeId = 22, PayrollRunId = 2, EmployeeId = 2, GrossPay = 500, NetPay = 400 };
        payrollRunEmployee.NetPayLines.Add(new NetPayLine { Amount = 400, PaymentMethod = "Check" });
        ctx.PayrollRunEmployees.Add(payrollRunEmployee);
        await ctx.SaveChangesAsync();

        var service = new PayStubService(ctx);
        var payStub = await service.GetPayStubByPayrollRunEmployeeIdAsync(22);

        Assert.Equal("Check", payStub.PaymentMethod);
        Assert.Equal("CHK-2-2", payStub.CheckNumberPlaceholder);
    }

    [Fact]
    public async Task GetPayStubAsync_Throws_WhenPayrollRunEmployeeMissing()
    {
        var ctx = CreateInMemoryContext("paystub_missing");
        var service = new PayStubService(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetPayStubAsync(999, 999));
    }
}
