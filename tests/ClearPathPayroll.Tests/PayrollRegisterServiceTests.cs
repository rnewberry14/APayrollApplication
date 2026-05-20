using System;
using System.Linq;
using System.Threading.Tasks;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollRegisterServiceTests
{
    private PayrollDbContext CreateInMemoryContext(string name)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task GetPayrollRegisterAsync_FiltersByCompanyAndDateRange()
    {
        var ctx = CreateInMemoryContext("register_filters");

        var companyA = new Company { CompanyId = 1, LegalName = "Alpha Co", FEIN = "123456789", PrimaryAddress = "1 Main St", City = "City", State = "CA", ZipCode = "90001", IsActive = true };
        var companyB = new Company { CompanyId = 2, LegalName = "Beta Co", FEIN = "987654321", PrimaryAddress = "2 Main St", City = "City", State = "CA", ZipCode = "90001", IsActive = true };
        ctx.Companies.AddRange(companyA, companyB);

        var employeeA = new Employee { EmployeeId = 1, CompanyId = 1, FirstName = "John", LastName = "Doe", SSNLast4 = "1111", DateOfBirth = DateTime.UtcNow.AddYears(-28), HireDate = DateTime.UtcNow.AddYears(-3), ResidenceAddress = "123 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Salary };
        var employeeB = new Employee { EmployeeId = 2, CompanyId = 2, FirstName = "Jane", LastName = "Smith", SSNLast4 = "2222", DateOfBirth = DateTime.UtcNow.AddYears(-32), HireDate = DateTime.UtcNow.AddYears(-2), ResidenceAddress = "456 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Hourly };
        ctx.Employees.AddRange(employeeA, employeeB);

        var payrollRunA = new PayrollRun { PayrollRunId = 1, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow.AddDays(-1), PayDate = DateTime.Today.AddDays(-1), Status = PayrollStatus.Approved, TotalGrossPay = 1000m, TotalEmployeeTaxes = 150m, TotalEmployerTaxes = 100m, TotalDeductions = 50m, TotalNetPay = 800m };
        var payrollRunB = new PayrollRun { PayrollRunId = 2, CompanyId = 2, PayScheduleId = 1, PayPeriodStart = DateTime.UtcNow.AddDays(-14), PayPeriodEnd = DateTime.UtcNow.AddDays(-1), PayDate = DateTime.Today, Status = PayrollStatus.Approved, TotalGrossPay = 500m, TotalEmployeeTaxes = 75m, TotalEmployerTaxes = 40m, TotalDeductions = 25m, TotalNetPay = 400m };
        ctx.PayrollRuns.AddRange(payrollRunA, payrollRunB);

        var preA = new PayrollRunEmployee { PayrollRunEmployeeId = 1, PayrollRunId = 1, EmployeeId = 1, GrossPay = 1000m, TotalTaxes = 150m, TotalDeductions = 50m, NetPay = 800m };
        var preB = new PayrollRunEmployee { PayrollRunEmployeeId = 2, PayrollRunId = 2, EmployeeId = 2, GrossPay = 500m, TotalTaxes = 75m, TotalDeductions = 25m, NetPay = 400m };
        ctx.PayrollRunEmployees.AddRange(preA, preB);

        await ctx.SaveChangesAsync();

        var service = new PayrollRegisterService(ctx);
        var report = await service.GetPayrollRegisterAsync(1, DateTime.Today.AddDays(-7), DateTime.Today);

        Assert.Single(report.Runs);
        Assert.Equal("Alpha Co", report.CompanyName);
        Assert.Equal(1000m, report.TotalGrossWages);
        Assert.Equal(150m, report.TotalEmployeeTaxes);
        Assert.Equal(100m, report.TotalEmployerTaxes);
        Assert.Equal(50m, report.TotalDeductions);
        Assert.Equal(800m, report.TotalNetPay);
        Assert.Single(report.Runs[0].EmployeeSummaries);
        Assert.Equal("John Doe", report.Runs[0].EmployeeSummaries[0].EmployeeName);
    }

    [Fact]
    public void BuildPayrollRegisterCsv_IncludesHeadersAndTotals()
    {
        var report = new PayrollRegisterModel
        {
            CompanyName = "Test Co",
            PayDateStart = new DateTime(2026, 1, 1),
            PayDateEnd = new DateTime(2026, 1, 31),
            GeneratedAt = new DateTime(2026, 1, 31, 12, 0, 0),
            Runs = new System.Collections.Generic.List<PayrollRegisterRun>
            {
                new PayrollRegisterRun
                {
                    PayrollRunId = 10,
                    CompanyName = "Test Co",
                    PayDate = new DateTime(2026, 1, 15),
                    PayPeriodStart = new DateTime(2026, 1, 1),
                    PayPeriodEnd = new DateTime(2026, 1, 15),
                    Status = PayrollStatus.Approved,
                    TotalGrossPay = 1000m,
                    TotalEmployeeTaxes = 150m,
                    TotalEmployerTaxes = 120m,
                    TotalDeductions = 80m,
                    TotalNetPay = 770m,
                    EmployeeSummaries = new System.Collections.Generic.List<PayrollRegisterEmployeeSummary>
                    {
                        new PayrollRegisterEmployeeSummary
                        {
                            EmployeeId = 1,
                            EmployeeName = "John Doe",
                            GrossPay = 1000m,
                            TotalTaxes = 150m,
                            TotalDeductions = 80m,
                            NetPay = 770m
                        }
                    }
                }
            }
        };

        var testContext = CreateInMemoryContext("register_csv");
        var service = new PayrollRegisterService(testContext);
        var csv = service.BuildPayrollRegisterCsv(report);

        Assert.Contains("Payroll Register Report", csv);
        Assert.Contains("Company,Test Co", csv);
        Assert.Contains("John Doe", csv);
        Assert.Contains("1000.00", csv);
        Assert.Contains("770.00", csv);
    }
}
