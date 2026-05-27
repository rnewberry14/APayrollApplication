using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearPathPayroll.Tests;

public class EmployeeSetupServiceTests
{
    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    [Fact]
    public async Task SaveEmployeePayrollProfileAsync_PersistsTokenizedDirectDepositAndFieldValues()
    {
        await using var context = CreateContext("employee_setup_save");
        var service = new EmployeeService(context, new TestEncryptionService());
        var employee = CreateEmployee();
        employee.BankAccounts.Add(CreateBankAccount());
        employee.PayrollFields.Add(new EmployeePayrollField { FieldName = "Local field", FieldValue = "Local value" });

        var saved = await service.SaveEmployeePayrollProfileAsync(employee);

        var reloaded = await context.Employees
            .Include(e => e.BankAccounts)
            .Include(e => e.PayrollFields)
            .SingleAsync(e => e.EmployeeId == saved.EmployeeId);

        Assert.Single(reloaded.BankAccounts);
        Assert.Equal("routing-token-placeholder", reloaded.BankAccounts.First().RoutingNumberToken);
        Assert.Equal("6789", reloaded.BankAccounts.First().Last4);
        Assert.Null(reloaded.FullSSNEncryptedPlaceholder);
        Assert.Single(reloaded.PayrollFields);
    }

    [Fact]
    public async Task SaveEmployeePayrollProfileAsync_RejectsRawBankNumbers()
    {
        await using var context = CreateContext("employee_setup_rejects_raw_bank");
        var service = new EmployeeService(context, new TestEncryptionService());
        var employee = CreateEmployee();
        var account = CreateBankAccount();
        account.RoutingNumberToken = "123456789";
        account.AccountNumberToken = "1234567890";
        employee.BankAccounts.Add(account);

        await Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(() =>
            service.SaveEmployeePayrollProfileAsync(employee));
    }

    private static Employee CreateEmployee()
    {
        return new Employee
        {
            CompanyId = 1,
            FirstName = "Jordan",
            LastName = "Local",
            SSNLast4 = "1234",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2026, 1, 1),
            EmploymentStatus = EmploymentStatus.Active,
            WorkerType = WorkerType.W2Employee,
            PayType = PayType.Hourly,
            HourlyRate = 25m,
            ResidenceAddress = "100 Local Street",
            Address1 = "100 Local Street",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102"
        };
    }

    private static EmployeeBankAccount CreateBankAccount()
    {
        return new EmployeeBankAccount
        {
            PriorityOrder = 1,
            BankName = "Local Test Bank",
            AccountType = BankAccountType.Checking,
            RoutingNumberToken = "routing-token-placeholder",
            AccountNumberToken = "account-token-placeholder",
            Last4 = "6789",
            DepositType = DirectDepositDepositType.Remainder,
            IsRemainderAccount = true,
            VerificationStatus = VerificationStatus.Pending,
            PrenoteStatus = PrenoteStatus.NotStarted,
            IsActive = true
        };
    }

    private sealed class TestEncryptionService : IEncryptionService
    {
        public Task<string> EncryptAsync(string plainText) => Task.FromResult(plainText);

        public Task<string> DecryptAsync(string encryptedText) => Task.FromResult(encryptedText);
    }
}
