using ClearPathPayroll.Configuration;
using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ClearPathPayroll.Services;

public sealed class DemoDataSeederResult
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public int PayScheduleId { get; set; }
    public int PayrollRunId { get; set; }
    public bool AlreadyExists { get; set; }
    public string Message { get; set; } = string.Empty;
}

public sealed class DemoDataSeeder
{
    public const string DemoCompanyName = "Demo Company LLC";
    public const string DemoCompanyFein = "00-0000000";
    private const string DemoUserId = "local-demo-seeder";

    private readonly PayrollDbContext _context;
    private readonly IHostEnvironment _environment;
    private readonly LimitedLiabilityModeOptions _limitedLiabilityOptions;

    public DemoDataSeeder(
        PayrollDbContext context,
        IHostEnvironment environment,
        IOptions<LimitedLiabilityModeOptions> limitedLiabilityOptions)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _limitedLiabilityOptions = limitedLiabilityOptions?.Value ?? throw new ArgumentNullException(nameof(limitedLiabilityOptions));
    }

    private bool IsAllowed => _environment.IsDevelopment() || _limitedLiabilityOptions.Enabled;

    public async Task<DemoDataSeederResult> CreateDemoDataAsync()
    {
        EnsureAllowed();

        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.LegalName == DemoCompanyName);

        var alreadyExists = company != null;
        var repaired = false;

        if (company == null)
        {
            company = new Company
            {
                LegalName = DemoCompanyName,
                FEIN = DemoCompanyFein,
                PrimaryAddress = "100 Demo Center",
                City = "Oklahoma City",
                State = "OK",
                ZipCode = "73102",
                Phone = "555-0100",
                Email = "payroll.demo@example.local",
                PayrollContactName = "Demo Payroll Admin",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }
        else
        {
            repaired = EnsureCompanyDefaults(company);
            if (repaired)
            {
                await _context.SaveChangesAsync();
            }
        }

        var employees = await EnsureEmployeesAsync(company.CompanyId);
        if (employees.CreatedOrRepaired)
        {
            repaired = true;
        }

        var fundingAccount = await EnsureCompanyFundingAccountAsync(company.CompanyId);
        if (fundingAccount.CreatedOrRepaired)
        {
            repaired = true;
        }

        var employeeBankAccountsRepaired = await EnsureEmployeeBankAccountsAsync(employees.Employees);
        if (employeeBankAccountsRepaired)
        {
            repaired = true;
        }

        var paySchedule = await EnsurePayScheduleAsync(company.CompanyId);
        if (paySchedule.CreatedOrRepaired)
        {
            repaired = true;
        }

        var payrollRun = await EnsureDraftPayrollRunAsync(company.CompanyId, paySchedule.PaySchedule.PayScheduleId, paySchedule.PaySchedule);
        if (payrollRun.CreatedOrRepaired)
        {
            repaired = true;
        }

        var payrollEmployeesRepaired = await EnsurePayrollRunEmployeesAsync(payrollRun.PayrollRun.PayrollRunId, employees.Employees);
        if (payrollEmployeesRepaired)
        {
            repaired = true;
        }

        var message = alreadyExists
            ? repaired
                ? "Demo data repaired and is ready for local prototype testing. Demo data is fake and must not be used for real payroll."
                : "Demo data already exists and is ready for local prototype testing."
            : "Demo data created. Demo data is fake and must not be used for real payroll.";

        return new DemoDataSeederResult
        {
            CompanyId = company.CompanyId,
            CompanyName = company.LegalName,
            EmployeeCount = employees.Employees.Count,
            PayScheduleId = paySchedule.PaySchedule.PayScheduleId,
            PayrollRunId = payrollRun.PayrollRun.PayrollRunId,
            AlreadyExists = alreadyExists && !repaired,
            Message = message
        };
    }

    public async Task<bool> ClearDemoDataAsync()
    {
        EnsureAllowed();

        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.LegalName == DemoCompanyName);

        if (company == null)
        {
            return false;
        }

        var companyId = company.CompanyId;
        var payrollRuns = await _context.PayrollRuns
            .Where(p => p.CompanyId == companyId)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(e => e.EarningLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(e => e.DeductionLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(e => e.TaxLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(e => e.NetPayLines)
            .ToListAsync();

        foreach (var payrollRun in payrollRuns)
        {
            foreach (var payrollEmployee in payrollRun.PayrollRunEmployees)
            {
                _context.EarningLines.RemoveRange(payrollEmployee.EarningLines);
                _context.DeductionLines.RemoveRange(payrollEmployee.DeductionLines);
                _context.TaxLines.RemoveRange(payrollEmployee.TaxLines);
                _context.NetPayLines.RemoveRange(payrollEmployee.NetPayLines);
            }

            _context.PayrollRunEmployees.RemoveRange(payrollRun.PayrollRunEmployees);
            _context.EmployerTaxLines.RemoveRange(_context.EmployerTaxLines.Where(t => t.PayrollRunId == payrollRun.PayrollRunId));
            _context.AuditLogEntries.RemoveRange(_context.AuditLogEntries.Where(a => a.PayrollRunId == payrollRun.PayrollRunId));
        }

        _context.PayrollRuns.RemoveRange(payrollRuns);

        var employeeIds = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .Select(e => e.EmployeeId)
            .ToListAsync();

        _context.DirectDepositItems.RemoveRange(_context.DirectDepositItems.Where(i => employeeIds.Contains(i.EmployeeId)));
        _context.DirectDepositBatches.RemoveRange(_context.DirectDepositBatches.Where(b => b.CompanyId == companyId));
        _context.EmployeeBankAccounts.RemoveRange(_context.EmployeeBankAccounts.Where(b => employeeIds.Contains(b.EmployeeId)));
        _context.CompanyFundingAccounts.RemoveRange(_context.CompanyFundingAccounts.Where(f => f.CompanyId == companyId));
        _context.PaySchedules.RemoveRange(_context.PaySchedules.Where(p => p.CompanyId == companyId));
        _context.Employees.RemoveRange(_context.Employees.Where(e => e.CompanyId == companyId));
        _context.Companies.Remove(company);

        await _context.SaveChangesAsync();
        return true;
    }

    private void EnsureAllowed()
    {
        if (!IsAllowed)
        {
            throw new InvalidOperationException("Demo data seeding is only available in Development or Local-Only Mode.");
        }
    }

    private static bool EnsureCompanyDefaults(Company company)
    {
        var changed = false;

        if (company.FEIN != DemoCompanyFein)
        {
            company.FEIN = DemoCompanyFein;
            changed = true;
        }

        if (company.State != "OK")
        {
            company.State = "OK";
            changed = true;
        }

        if (!company.IsActive)
        {
            company.IsActive = true;
            changed = true;
        }

        return changed;
    }

    private async Task<(List<Employee> Employees, bool CreatedOrRepaired)> EnsureEmployeesAsync(int companyId)
    {
        var changed = false;
        var templates = CreateEmployees(companyId);
        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();

        foreach (var template in templates)
        {
            var employee = employees.FirstOrDefault(e => e.Email == template.Email)
                ?? employees.FirstOrDefault(e => e.FirstName == template.FirstName && e.LastName == template.LastName);

            if (employee == null)
            {
                _context.Employees.Add(template);
                employees.Add(template);
                changed = true;
                continue;
            }

            changed |= EnsureEmployeeDefaults(employee, template);
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        var refreshedEmployees = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();
        employees = templates
            .Select(template => refreshedEmployees.First(e => e.Email == template.Email || (e.FirstName == template.FirstName && e.LastName == template.LastName)))
            .ToList();

        return (employees, changed);
    }

    private static bool EnsureEmployeeDefaults(Employee employee, Employee template)
    {
        var changed = false;

        if (employee.SSNLast4 != template.SSNLast4)
        {
            employee.SSNLast4 = template.SSNLast4;
            changed = true;
        }

        if (employee.FullSSNEncryptedPlaceholder != null)
        {
            employee.FullSSNEncryptedPlaceholder = null;
            changed = true;
        }

        if (employee.EmploymentStatus != EmploymentStatus.Active)
        {
            employee.EmploymentStatus = EmploymentStatus.Active;
            changed = true;
        }

        if (employee.PayType != template.PayType)
        {
            employee.PayType = template.PayType;
            changed = true;
        }

        if (employee.HourlyRate != template.HourlyRate)
        {
            employee.HourlyRate = template.HourlyRate;
            changed = true;
        }

        if (employee.AnnualSalary != template.AnnualSalary)
        {
            employee.AnnualSalary = template.AnnualSalary;
            changed = true;
        }

        if (employee.State != "OK")
        {
            employee.State = "OK";
            changed = true;
        }

        return changed;
    }

    private async Task<(CompanyFundingAccount FundingAccount, bool CreatedOrRepaired)> EnsureCompanyFundingAccountAsync(int companyId)
    {
        var fundingAccount = await _context.CompanyFundingAccounts
            .Where(f => f.CompanyId == companyId)
            .OrderByDescending(f => f.IsPrimary)
            .ThenBy(f => f.CompanyFundingAccountId)
            .FirstOrDefaultAsync();

        if (fundingAccount == null)
        {
            fundingAccount = new CompanyFundingAccount
            {
                CompanyId = companyId,
                BankName = "Demo Funding Bank",
                AccountType = BankAccountType.Checking,
                RoutingNumberToken = "fake-company-routing-token",
                AccountNumberToken = "fake-company-account-token",
                Last4 = "9000",
                Description = "Fake demo company funding account",
                IsPrimary = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByUserId = DemoUserId
            };

            _context.CompanyFundingAccounts.Add(fundingAccount);
            await _context.SaveChangesAsync();
            return (fundingAccount, true);
        }

        var changed = false;
        if (!fundingAccount.IsPrimary)
        {
            fundingAccount.IsPrimary = true;
            changed = true;
        }

        if (!fundingAccount.IsActive)
        {
            fundingAccount.IsActive = true;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(fundingAccount.RoutingNumberToken))
        {
            fundingAccount.RoutingNumberToken = "fake-company-routing-token";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(fundingAccount.AccountNumberToken))
        {
            fundingAccount.AccountNumberToken = "fake-company-account-token";
            changed = true;
        }

        if (changed)
        {
            fundingAccount.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return (fundingAccount, changed);
    }

    private async Task<bool> EnsureEmployeeBankAccountsAsync(IEnumerable<Employee> employees)
    {
        var changed = false;

        foreach (var employee in employees)
        {
            var bankAccount = await _context.EmployeeBankAccounts
                .Where(b => b.EmployeeId == employee.EmployeeId)
                .OrderByDescending(b => b.IsActive)
                .ThenBy(b => b.EmployeeBankAccountId)
                .FirstOrDefaultAsync();

            if (bankAccount == null)
            {
                _context.EmployeeBankAccounts.Add(new EmployeeBankAccount
                {
                    EmployeeId = employee.EmployeeId,
                    BankName = "Demo Employee Bank",
                    AccountType = BankAccountType.Checking,
                    RoutingNumberToken = $"fake-routing-token-{employee.SSNLast4}",
                    AccountNumberToken = $"fake-account-token-{employee.SSNLast4}",
                    Last4 = employee.SSNLast4,
                    VerificationStatus = VerificationStatus.Verified,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedByUserId = DemoUserId
                });
                changed = true;
                continue;
            }

            if (!bankAccount.IsActive)
            {
                bankAccount.IsActive = true;
                changed = true;
            }

            if (bankAccount.VerificationStatus != VerificationStatus.Verified)
            {
                bankAccount.VerificationStatus = VerificationStatus.Verified;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(bankAccount.RoutingNumberToken))
            {
                bankAccount.RoutingNumberToken = $"fake-routing-token-{employee.SSNLast4}";
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(bankAccount.AccountNumberToken))
            {
                bankAccount.AccountNumberToken = $"fake-account-token-{employee.SSNLast4}";
                changed = true;
            }

            if (changed)
            {
                bankAccount.UpdatedAt = DateTime.UtcNow;
            }
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        return changed;
    }

    private async Task<(PaySchedule PaySchedule, bool CreatedOrRepaired)> EnsurePayScheduleAsync(int companyId)
    {
        var paySchedule = await _context.PaySchedules
            .Where(p => p.CompanyId == companyId && p.Name == "Biweekly Demo Payroll")
            .FirstOrDefaultAsync();

        if (paySchedule == null)
        {
            paySchedule = new PaySchedule
            {
                CompanyId = companyId,
                Name = "Biweekly Demo Payroll",
                Frequency = PayFrequency.Biweekly,
                NextPayDate = DateTime.UtcNow.Date.AddDays(7),
                NextPeriodStartDate = DateTime.UtcNow.Date.AddDays(-13),
                NextPeriodEndDate = DateTime.UtcNow.Date,
                IsActive = true
            };

            _context.PaySchedules.Add(paySchedule);
            await _context.SaveChangesAsync();
            return (paySchedule, true);
        }

        var changed = false;
        if (!paySchedule.IsActive)
        {
            paySchedule.IsActive = true;
            changed = true;
        }

        if (paySchedule.Frequency != PayFrequency.Biweekly)
        {
            paySchedule.Frequency = PayFrequency.Biweekly;
            changed = true;
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        return (paySchedule, changed);
    }

    private async Task<(PayrollRun PayrollRun, bool CreatedOrRepaired)> EnsureDraftPayrollRunAsync(int companyId, int payScheduleId, PaySchedule paySchedule)
    {
        var payrollRun = await _context.PayrollRuns
            .Where(p => p.CompanyId == companyId && p.Status == PayrollStatus.Draft)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        if (payrollRun == null)
        {
            payrollRun = new PayrollRun
            {
                CompanyId = companyId,
                PayScheduleId = payScheduleId,
                PayPeriodStart = paySchedule.NextPeriodStartDate,
                PayPeriodEnd = paySchedule.NextPeriodEndDate,
                PayDate = paySchedule.NextPayDate,
                Status = PayrollStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = DemoUserId
            };

            _context.PayrollRuns.Add(payrollRun);
            await _context.SaveChangesAsync();
            return (payrollRun, true);
        }

        var changed = false;
        if (payrollRun.PayScheduleId != payScheduleId)
        {
            payrollRun.PayScheduleId = payScheduleId;
            changed = true;
        }

        if (payrollRun.PayPeriodStart == default)
        {
            payrollRun.PayPeriodStart = paySchedule.NextPeriodStartDate;
            changed = true;
        }

        if (payrollRun.PayPeriodEnd == default)
        {
            payrollRun.PayPeriodEnd = paySchedule.NextPeriodEndDate;
            changed = true;
        }

        if (payrollRun.PayDate == default)
        {
            payrollRun.PayDate = paySchedule.NextPayDate;
            changed = true;
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        return (payrollRun, changed);
    }

    private async Task<bool> EnsurePayrollRunEmployeesAsync(int payrollRunId, IReadOnlyList<Employee> employees)
    {
        var existing = await _context.PayrollRunEmployees
            .Include(e => e.EarningLines)
            .Where(e => e.PayrollRunId == payrollRunId)
            .ToListAsync();

        var changed = false;
        var templates = CreatePayrollRunEmployees(payrollRunId, employees);

        foreach (var template in templates)
        {
            var payrollEmployee = existing.FirstOrDefault(e => e.EmployeeId == template.EmployeeId);
            if (payrollEmployee == null)
            {
                _context.PayrollRunEmployees.Add(template);
                changed = true;
                continue;
            }

            if (!payrollEmployee.EarningLines.Any())
            {
                foreach (var earningLine in template.EarningLines)
                {
                    payrollEmployee.EarningLines.Add(new EarningLine
                    {
                        Description = earningLine.Description,
                        Hours = earningLine.Hours,
                        Rate = earningLine.Rate,
                        Amount = earningLine.Amount
                    });
                }

                changed = true;
            }
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        return changed;
    }

    private static List<Employee> CreateEmployees(int companyId)
    {
        return new List<Employee>
        {
            new()
            {
                CompanyId = companyId,
                FirstName = "Harper",
                LastName = "Hourly",
                SSNLast4 = "1001",
                FullSSNEncryptedPlaceholder = null,
                DateOfBirth = new DateTime(1991, 2, 10),
                HireDate = DateTime.UtcNow.Date.AddYears(-2),
                EmploymentStatus = EmploymentStatus.Active,
                PayType = PayType.Hourly,
                HourlyRate = 24.50m,
                ResidenceAddress = "101 Demo Street",
                City = "Oklahoma City",
                State = "OK",
                ZipCode = "73102",
                Email = "harper.hourly@example.local"
            },
            new()
            {
                CompanyId = companyId,
                FirstName = "Sam",
                LastName = "Salary",
                SSNLast4 = "1002",
                FullSSNEncryptedPlaceholder = null,
                DateOfBirth = new DateTime(1987, 8, 4),
                HireDate = DateTime.UtcNow.Date.AddYears(-4),
                EmploymentStatus = EmploymentStatus.Active,
                PayType = PayType.Salary,
                AnnualSalary = 62400m,
                ResidenceAddress = "202 Demo Avenue",
                City = "Oklahoma City",
                State = "OK",
                ZipCode = "73103",
                Email = "sam.salary@example.local"
            },
            new()
            {
                CompanyId = companyId,
                FirstName = "Taylor",
                LastName = "Tips",
                SSNLast4 = "1003",
                FullSSNEncryptedPlaceholder = null,
                DateOfBirth = new DateTime(1995, 5, 20),
                HireDate = DateTime.UtcNow.Date.AddMonths(-10),
                EmploymentStatus = EmploymentStatus.Active,
                PayType = PayType.Hourly,
                HourlyRate = 12.00m,
                ResidenceAddress = "303 Demo Road",
                City = "Oklahoma City",
                State = "OK",
                ZipCode = "73104",
                Email = "taylor.tips@example.local"
            }
        };
    }

    private static List<PayrollRunEmployee> CreatePayrollRunEmployees(int payrollRunId, IReadOnlyList<Employee> employees)
    {
        var hourly = new PayrollRunEmployee
        {
            PayrollRunId = payrollRunId,
            EmployeeId = employees[0].EmployeeId,
            GrossPay = 0m,
            TotalDeductions = 0m,
            TotalTaxes = 0m,
            NetPay = 0m
        };
        hourly.EarningLines.Add(new EarningLine
        {
            Description = "Regular Hours",
            Hours = 40m,
            Rate = 24.50m,
            Amount = 980m
        });

        var salary = new PayrollRunEmployee
        {
            PayrollRunId = payrollRunId,
            EmployeeId = employees[1].EmployeeId,
            GrossPay = 0m,
            TotalDeductions = 0m,
            TotalTaxes = 0m,
            NetPay = 0m
        };
        salary.EarningLines.Add(new EarningLine
        {
            Description = "Salary",
            Amount = 2400m
        });

        var tipped = new PayrollRunEmployee
        {
            PayrollRunId = payrollRunId,
            EmployeeId = employees[2].EmployeeId,
            GrossPay = 0m,
            TotalDeductions = 0m,
            TotalTaxes = 0m,
            NetPay = 0m
        };
        tipped.EarningLines.Add(new EarningLine
        {
            Description = "Regular Hours - Tipped Employee Placeholder",
            Hours = 30m,
            Rate = 12.00m,
            Amount = 360m
        });
        tipped.EarningLines.Add(new EarningLine
        {
            Description = "Tips Placeholder",
            Amount = 180m
        });

        return new List<PayrollRunEmployee> { hourly, salary, tipped };
    }
}
