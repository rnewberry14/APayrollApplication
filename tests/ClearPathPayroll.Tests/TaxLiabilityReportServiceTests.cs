using System;
using System.Linq;
using System.Threading.Tasks;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class TaxLiabilityReportServiceTests
{
    private PayrollDbContext CreateInMemoryContext(string name)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task GetTaxLiabilityReportAsync_GroupsEmployeeAndEmployerTaxesByType()
    {
        var ctx = CreateInMemoryContext("tax_liability_groups");

        var company = new Company { CompanyId = 1, LegalName = "Alpha Co", FEIN = "123456789", PrimaryAddress = "1 Main St", City = "City", State = "CA", ZipCode = "90001", IsActive = true };
        ctx.Companies.Add(company);

        var employee = new Employee { EmployeeId = 1, CompanyId = 1, FirstName = "John", LastName = "Doe", SSNLast4 = "1111", DateOfBirth = DateTime.UtcNow.AddYears(-30), HireDate = DateTime.UtcNow.AddYears(-2), ResidenceAddress = "123 Main St", City = "City", State = "CA", ZipCode = "90001", PayType = PayType.Salary };
        ctx.Employees.Add(employee);

        var payrollRun = new PayrollRun { PayrollRunId = 1, CompanyId = 1, PayScheduleId = 1, PayPeriodStart = new DateTime(2026, 5, 1), PayPeriodEnd = new DateTime(2026, 5, 15), PayDate = new DateTime(2026, 5, 15), Status = PayrollStatus.Approved, TotalGrossPay = 2000m, TotalEmployeeTaxes = 250m, TotalEmployerTaxes = 120m, TotalDeductions = 80m, TotalNetPay = 1670m };
        ctx.PayrollRuns.Add(payrollRun);

        var payrollRunEmployee = new PayrollRunEmployee { PayrollRunEmployeeId = 1, PayrollRunId = 1, EmployeeId = 1, GrossPay = 2000m, TotalTaxes = 250m, TotalDeductions = 80m, NetPay = 1670m };
        ctx.PayrollRunEmployees.Add(payrollRunEmployee);

        ctx.TaxLines.AddRange(
            new TaxLine { TaxLineId = 1, PayrollRunEmployeeId = 1, TaxType = "Federal Income", Amount = 150m },
            new TaxLine { TaxLineId = 2, PayrollRunEmployeeId = 1, TaxType = "Social Security", Amount = 62m },
            new TaxLine { TaxLineId = 3, PayrollRunEmployeeId = 1, TaxType = "Medicare", Amount = 38m }
        );

        ctx.EmployerTaxLines.AddRange(
            new EmployerTaxLine { EmployerTaxLineId = 1, PayrollRunId = 1, TaxType = "Social Security", Amount = 62m },
            new EmployerTaxLine { EmployerTaxLineId = 2, PayrollRunId = 1, TaxType = "Medicare", Amount = 38m }
        );

        await ctx.SaveChangesAsync();

        var service = new TaxLiabilityReportService(ctx);
        var report = await service.GetTaxLiabilityReportAsync(1, new DateTime(2026, 5, 1), new DateTime(2026, 5, 31));

        Assert.Single(report.Runs);
        var run = report.Runs.Single();
        Assert.Equal("Alpha Co", report.CompanyName);
        Assert.Equal(3, run.EmployeeWithholdingGroups.Count);
        Assert.Equal(2, run.EmployerTaxGroups.Count);
        Assert.Equal(250m, run.TotalEmployeeWithholding);
        Assert.Equal(100m, run.TotalEmployerTaxes);
        Assert.Equal(350m, run.TotalTaxLiability);
        Assert.Equal(350m, report.TotalTaxLiability);
        Assert.All(run.EmployeeWithholdingGroups, group => Assert.True(group.Amount > 0));
    }

    [Fact]
    public void BuildTaxLiabilityCsv_IncludesPlaceholderColumnsAndTotals()
    {
        var report = new TaxLiabilityReportModel
        {
            CompanyName = "Alpha Co",
            PayDateStart = new DateTime(2026, 5, 1),
            PayDateEnd = new DateTime(2026, 5, 31),
            GeneratedAt = new DateTime(2026, 5, 31, 12, 0, 0),
            Runs = new System.Collections.Generic.List<TaxLiabilityRun>
            {
                new TaxLiabilityRun
                {
                    PayrollRunId = 1,
                    CompanyName = "Alpha Co",
                    PayDate = new DateTime(2026, 5, 15),
                    PayPeriodStart = new DateTime(2026, 5, 1),
                    PayPeriodEnd = new DateTime(2026, 5, 15),
                    DueDatePlaceholder = "TBD",
                    PaymentStatusPlaceholder = "Pending review",
                    EmployeeWithholdingGroups = new System.Collections.Generic.List<TaxLiabilityGroup>
                    {
                        new TaxLiabilityGroup { TaxType = "Federal Income", Amount = 150m }
                    },
                    EmployerTaxGroups = new System.Collections.Generic.List<TaxLiabilityGroup>
                    {
                        new TaxLiabilityGroup { TaxType = "Medicare", Amount = 38m }
                    },
                    TotalEmployeeWithholding = 150m,
                    TotalEmployerTaxes = 38m,
                    TotalTaxLiability = 188m
                }
            }
        };

        var testContext = CreateInMemoryContext("tax_liability_csv");
        var service = new TaxLiabilityReportService(testContext);
        var csv = service.BuildTaxLiabilityCsv(report);

        Assert.Contains("Tax Liability Report", csv);
        Assert.Contains("Due Date", csv);
        Assert.Contains("Payment Status", csv);
        Assert.Contains("Employee Withholding", csv);
        Assert.Contains("Employer Tax", csv);
        Assert.Contains("188.00", csv);
    }
}
