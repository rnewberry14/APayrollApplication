using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PaycheckPrintServiceTests
{
    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task GetPrintablePaycheckAsync_ReturnsPaycheckDataForCheckPayment()
    {
        await using var context = CreateContext("paycheck_print_check");
        await SeedPaycheckAsync(context, "Check");
        var service = new PaycheckPrintService(context);

        var paycheck = await service.GetPrintablePaycheckAsync(300);

        Assert.Equal("Demo Company LLC", paycheck.CompanyName);
        Assert.Contains("100 Local Way", paycheck.CompanyAddress);
        Assert.Equal("Avery Local", paycheck.EmployeeName);
        Assert.Contains("200 Employee Ave", paycheck.EmployeeAddress);
        Assert.Equal(1234.56m, paycheck.NetPayAmount);
        Assert.Equal("One thousand two hundred thirty-four and 56/100 dollars", paycheck.NetPayInWords);
        Assert.Equal("CHK-100-200", paycheck.CheckNumberPlaceholder);
        Assert.False(paycheck.IsDirectDeposit);
    }

    [Fact]
    public async Task GetPrintablePaycheckAsync_BlocksDirectDepositPayment()
    {
        await using var context = CreateContext("paycheck_print_direct_deposit");
        await SeedPaycheckAsync(context, "DirectDeposit");
        var service = new PaycheckPrintService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPrintablePaycheckAsync(300));
    }

    [Theory]
    [InlineData(0, "Zero and 00/100 dollars")]
    [InlineData(25.01, "Twenty-five and 01/100 dollars")]
    [InlineData(1000000.99, "One million and 99/100 dollars")]
    public void ConvertMoneyToWords_ReturnsPrintableAmount(decimal amount, string expected)
    {
        Assert.Equal(expected, PaycheckPrintService.ConvertMoneyToWords(amount));
    }

    private static async Task SeedPaycheckAsync(PayrollDbContext context, string paymentMethod)
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
            Address1 = "200 Employee Ave",
            ResidenceAddress = "200 Employee Ave",
            City = "Tulsa",
            State = "OK",
            ZipCode = "74103"
        });
        context.PayrollRuns.Add(new PayrollRun
        {
            PayrollRunId = 100,
            CompanyId = 1,
            PayScheduleId = 10,
            PayPeriodStart = new DateTime(2026, 5, 16),
            PayPeriodEnd = new DateTime(2026, 5, 31),
            PayDate = new DateTime(2026, 6, 5),
            Status = PayrollStatus.Calculated
        });

        var check = new PayrollRunEmployee
        {
            PayrollRunEmployeeId = 300,
            PayrollRunId = 100,
            EmployeeId = 200,
            GrossPay = 1500m,
            TotalTaxes = 200m,
            TotalDeductions = 65.44m,
            NetPay = 1234.56m
        };
        check.NetPayLines.Add(new NetPayLine { NetPayLineId = 400, Amount = 1234.56m, PaymentMethod = paymentMethod });
        context.PayrollRunEmployees.Add(check);

        await context.SaveChangesAsync();
    }
}
