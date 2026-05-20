using ClearPathPayroll.Configuration;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace ClearPathPayroll.Services;

public class SeedDataResult
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public int PayrollRunId { get; set; }
    public bool AlreadyExists { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class SeedDataService
{
    private const string DemoCompanyName = "Demo Company (Local Prototype)";
    private const string DemoCompanyFein = "00-0000000";

    private readonly PayrollDbContext _context;
    private readonly IHostEnvironment _environment;
    private readonly PrototypeModeOptions _prototypeOptions;

    public SeedDataService(
        PayrollDbContext context,
        IHostEnvironment environment,
        IOptions<PrototypeModeOptions> prototypeOptions)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _prototypeOptions = prototypeOptions?.Value ?? throw new ArgumentNullException(nameof(prototypeOptions));
    }

    private bool IsAllowed => PrototypeModeHelper.ShouldUseLocalPrototypeMode(_environment, _prototypeOptions);

    public async Task<SeedDataResult> SeedDemoDataAsync()
    {
        if (!IsAllowed)
        {
            throw new InvalidOperationException("Demo seed data is only allowed in Development or Local Prototype Mode.");
        }

        var existingCompany = await _context.Companies
            .FirstOrDefaultAsync(c => c.LegalName == DemoCompanyName || c.FEIN == DemoCompanyFein);

        if (existingCompany != null)
        {
            var existingPayrollRun = await _context.PayrollRuns
                .FirstOrDefaultAsync(pr => pr.CompanyId == existingCompany.CompanyId && pr.Status == PayrollStatus.Draft);

            return new SeedDataResult
            {
                CompanyId = existingCompany.CompanyId,
                CompanyName = existingCompany.LegalName,
                EmployeeCount = await _context.Employees.CountAsync(e => e.CompanyId == existingCompany.CompanyId),
                PayrollRunId = existingPayrollRun?.PayrollRunId ?? 0,
                AlreadyExists = true,
                Message = "Demo data already exists in the local database."
            };
        }

        var company = new Company
        {
            LegalName = DemoCompanyName,
            FEIN = DemoCompanyFein,
            PrimaryAddress = "123 Demo Blvd.",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            Phone = "555-000-0000",
            Email = "demo.local@clearpathpayroll.local",
            PayrollContactName = "Demo Payroll Admin",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        var hourlyEmployee = new Employee
        {
            CompanyId = company.CompanyId,
            FirstName = "John",
            LastName = "Hourly",
            SSNLast4 = "0001",
            FullSSNEncryptedPlaceholder = "***-**-0001 (demo)",
            DateOfBirth = new DateTime(1992, 1, 1),
            HireDate = DateTime.UtcNow.AddYears(-2),
            EmploymentStatus = EmploymentStatus.Active,
            PayType = PayType.Hourly,
            HourlyRate = 25.00m,
            ResidenceAddress = "100 Demo Lane",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            Email = "john.hourly@demo.local"
        };

        var salaryEmployee = new Employee
        {
            CompanyId = company.CompanyId,
            FirstName = "Sally",
            LastName = "Salary",
            SSNLast4 = "0002",
            FullSSNEncryptedPlaceholder = "***-**-0002 (demo)",
            DateOfBirth = new DateTime(1988, 6, 15),
            HireDate = DateTime.UtcNow.AddYears(-3),
            EmploymentStatus = EmploymentStatus.Active,
            PayType = PayType.Salary,
            AnnualSalary = 52000.00m,
            ResidenceAddress = "200 Demo Avenue",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            Email = "sally.salary@demo.local"
        };

        var directDepositEmployee = new Employee
        {
            CompanyId = company.CompanyId,
            FirstName = "Danny",
            LastName = "Direct",
            SSNLast4 = "0003",
            FullSSNEncryptedPlaceholder = "***-**-0003 (demo)",
            DateOfBirth = new DateTime(1990, 3, 20),
            HireDate = DateTime.UtcNow.AddYears(-1),
            EmploymentStatus = EmploymentStatus.Active,
            PayType = PayType.Salary,
            AnnualSalary = 48000.00m,
            ResidenceAddress = "300 Demo Drive",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            Email = "danny.direct@demo.local"
        };

        _context.Employees.AddRange(hourlyEmployee, salaryEmployee, directDepositEmployee);
        await _context.SaveChangesAsync();

        var companyFundingAccount = new CompanyFundingAccount
        {
            CompanyId = company.CompanyId,
            BankName = "Demo Bank",
            AccountType = BankAccountType.Checking,
            RoutingNumberToken = "demo-routing-token-0000",
            AccountNumberToken = "demo-account-token-0000",
            Last4 = "0000",
            Description = "Demo company funding account",
            IsPrimary = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CompanyFundingAccounts.Add(companyFundingAccount);

        var employeeBankAccount = new EmployeeBankAccount
        {
            EmployeeId = directDepositEmployee.EmployeeId,
            BankName = "Demo Bank",
            AccountType = BankAccountType.Checking,
            RoutingNumberToken = "demo-routing-token-0000",
            AccountNumberToken = "demo-employee-account-token-0000",
            Last4 = "0000",
            VerificationStatus = VerificationStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.EmployeeBankAccounts.Add(employeeBankAccount);

        var paySchedule = new PaySchedule
        {
            CompanyId = company.CompanyId,
            Name = "Biweekly Demo Schedule",
            Frequency = PayFrequency.Biweekly,
            NextPayDate = DateTime.UtcNow.Date.AddDays(7),
            NextPeriodStartDate = DateTime.UtcNow.Date.AddDays(1),
            NextPeriodEndDate = DateTime.UtcNow.Date.AddDays(14),
            IsActive = true
        };

        _context.PaySchedules.Add(paySchedule);
        await _context.SaveChangesAsync();

        var payrollRun = new PayrollRun
        {
            CompanyId = company.CompanyId,
            PayScheduleId = paySchedule.PayScheduleId,
            PayPeriodStart = DateTime.UtcNow.Date.AddDays(-13),
            PayPeriodEnd = DateTime.UtcNow.Date,
            PayDate = DateTime.UtcNow.Date.AddDays(1),
            Status = PayrollStatus.Draft,
            TotalGrossPay = 0m,
            TotalEmployeeTaxes = 0m,
            TotalEmployerTaxes = 0m,
            TotalDeductions = 0m,
            TotalNetPay = 0m,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = "demo"
        };

        _context.PayrollRuns.Add(payrollRun);
        await _context.SaveChangesAsync();

        var hourlyPayroll = new PayrollRunEmployee
        {
            PayrollRunId = payrollRun.PayrollRunId,
            EmployeeId = hourlyEmployee.EmployeeId,
            GrossPay = 1000.00m,
            TotalDeductions = 60.00m,
            TotalTaxes = 150.00m,
            NetPay = 790.00m,
        };
        hourlyPayroll.EarningLines.Add(new EarningLine
        {
            Description = "Regular Hours",
            Hours = 40m,
            Rate = 25.00m,
            Amount = 1000.00m
        });
        hourlyPayroll.DeductionLines.Add(new DeductionLine
        {
            Description = "Health Insurance",
            Amount = 60.00m
        });
        hourlyPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Federal Income Tax",
            Amount = 80.00m
        });
        hourlyPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "State Income Tax",
            Amount = 30.00m
        });
        hourlyPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Social Security Tax",
            Amount = 30.00m
        });
        hourlyPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Medicare Tax",
            Amount = 10.00m
        });
        hourlyPayroll.NetPayLines.Add(new NetPayLine
        {
            Amount = 790.00m,
            PaymentMethod = "Check"
        });

        var salaryPayroll = new PayrollRunEmployee
        {
            PayrollRunId = payrollRun.PayrollRunId,
            EmployeeId = salaryEmployee.EmployeeId,
            GrossPay = 2000.00m,
            TotalDeductions = 120.00m,
            TotalTaxes = 300.00m,
            NetPay = 1580.00m,
        };
        salaryPayroll.EarningLines.Add(new EarningLine
        {
            Description = "Salary",
            Amount = 2000.00m
        });
        salaryPayroll.DeductionLines.Add(new DeductionLine
        {
            Description = "401(k)",
            Amount = 120.00m
        });
        salaryPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Federal Income Tax",
            Amount = 140.00m
        });
        salaryPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "State Income Tax",
            Amount = 60.00m
        });
        salaryPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Social Security Tax",
            Amount = 70.00m
        });
        salaryPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Medicare Tax",
            Amount = 30.00m
        });
        salaryPayroll.NetPayLines.Add(new NetPayLine
        {
            Amount = 1580.00m,
            PaymentMethod = "Check"
        });

        var directDepositPayroll = new PayrollRunEmployee
        {
            PayrollRunId = payrollRun.PayrollRunId,
            EmployeeId = directDepositEmployee.EmployeeId,
            GrossPay = 1846.15m,
            TotalDeductions = 80.00m,
            TotalTaxes = 280.00m,
            NetPay = 1486.15m,
        };
        directDepositPayroll.EarningLines.Add(new EarningLine
        {
            Description = "Salary",
            Amount = 1846.15m
        });
        directDepositPayroll.DeductionLines.Add(new DeductionLine
        {
            Description = "Health Savings",
            Amount = 80.00m
        });
        directDepositPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Federal Income Tax",
            Amount = 130.00m
        });
        directDepositPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "State Income Tax",
            Amount = 50.00m
        });
        directDepositPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Social Security Tax",
            Amount = 70.00m
        });
        directDepositPayroll.TaxLines.Add(new TaxLine
        {
            TaxType = "Medicare Tax",
            Amount = 30.00m
        });
        directDepositPayroll.NetPayLines.Add(new NetPayLine
        {
            Amount = 1486.15m,
            PaymentMethod = "DirectDeposit"
        });

        _context.PayrollRunEmployees.AddRange(hourlyPayroll, salaryPayroll, directDepositPayroll);

        payrollRun.TotalGrossPay = hourlyPayroll.GrossPay + salaryPayroll.GrossPay + directDepositPayroll.GrossPay;
        payrollRun.TotalDeductions = hourlyPayroll.TotalDeductions + salaryPayroll.TotalDeductions + directDepositPayroll.TotalDeductions;
        payrollRun.TotalEmployeeTaxes = hourlyPayroll.TotalTaxes + salaryPayroll.TotalTaxes + directDepositPayroll.TotalTaxes;
        payrollRun.TotalNetPay = hourlyPayroll.NetPay + salaryPayroll.NetPay + directDepositPayroll.NetPay;
        payrollRun.TotalEmployerTaxes = 240.00m;

        await _context.SaveChangesAsync();

        return new SeedDataResult
        {
            CompanyId = company.CompanyId,
            CompanyName = company.LegalName,
            EmployeeCount = 3,
            PayrollRunId = payrollRun.PayrollRunId,
            AlreadyExists = false,
            Message = "Demo data seeded successfully."
        };
    }

    public async Task<bool> ClearDemoDataAsync()
    {
        if (!IsAllowed)
        {
            throw new InvalidOperationException("Clearing demo data is only allowed in Development or Local Prototype Mode.");
        }

        // Find demo company by legal name or demo FEIN
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.LegalName == DemoCompanyName || c.FEIN == DemoCompanyFein);

        if (company == null)
        {
            return false;
        }

        // Remove related data safely
        var companyId = company.CompanyId;

        // Payroll runs and related lines
        var payrollRuns = await _context.PayrollRuns
            .Where(pr => pr.CompanyId == companyId)
            .Include(pr => pr.PayrollRunEmployees)
                .ThenInclude(pre => pre.EarningLines)
            .Include(pr => pr.PayrollRunEmployees)
                .ThenInclude(pre => pre.DeductionLines)
            .Include(pr => pr.PayrollRunEmployees)
                .ThenInclude(pre => pre.TaxLines)
            .Include(pr => pr.PayrollRunEmployees)
                .ThenInclude(pre => pre.NetPayLines)
            .ToListAsync();

        foreach (var pr in payrollRuns)
        {
            foreach (var pre in pr.PayrollRunEmployees)
            {
                _context.EarningLines.RemoveRange(pre.EarningLines);
                _context.DeductionLines.RemoveRange(pre.DeductionLines);
                _context.TaxLines.RemoveRange(pre.TaxLines);
                _context.NetPayLines.RemoveRange(pre.NetPayLines);
            }

            _context.PayrollRunEmployees.RemoveRange(pr.PayrollRunEmployees);
            _context.EmployerTaxLines.RemoveRange(_context.EmployerTaxLines.Where(e => e.PayrollRunId == pr.PayrollRunId));
            _context.AuditLogEntries.RemoveRange(_context.AuditLogEntries.Where(a => a.PayrollRunId == pr.PayrollRunId));
        }

        _context.PayrollRuns.RemoveRange(payrollRuns);

        // Employee bank accounts and direct deposit items/batches
        var employeeIds = await _context.Employees.Where(e => e.CompanyId == companyId).Select(e => e.EmployeeId).ToListAsync();

        var employeeBankAccounts = await _context.EmployeeBankAccounts.Where(b => employeeIds.Contains(b.EmployeeId)).ToListAsync();
        _context.DirectDepositItems.RemoveRange(_context.DirectDepositItems.Where(i => employeeBankAccounts.Select(b => b.EmployeeBankAccountId).Contains(i.EmployeeBankAccountId)));
        _context.EmployeeBankAccounts.RemoveRange(employeeBankAccounts);

        var companyFunding = await _context.CompanyFundingAccounts.Where(f => f.CompanyId == companyId).ToListAsync();
        _context.DirectDepositItems.RemoveRange(_context.DirectDepositItems.Where(i => companyFunding.Select(f => f.CompanyFundingAccountId).Contains(i.BatchId)));
        _context.CompanyFundingAccounts.RemoveRange(companyFunding);

        // Remove direct deposit batches for company
        _context.DirectDepositBatches.RemoveRange(_context.DirectDepositBatches.Where(b => b.CompanyId == companyId));

        // Remove employees
        var employees = await _context.Employees.Where(e => e.CompanyId == companyId).ToListAsync();
        _context.Employees.RemoveRange(employees);

        // Finally remove company
        _context.Companies.Remove(company);

        await _context.SaveChangesAsync();

        return true;
    }
}
