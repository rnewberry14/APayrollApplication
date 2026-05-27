using System.ComponentModel.DataAnnotations;
using ClearPathPayroll.Domain;
using Xunit;

namespace ClearPathPayroll.Tests;

public class EmployeePayrollProfileValidationTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    private static Employee CreateValidHourlyEmployee()
    {
        return new Employee
        {
            CompanyId = 1,
            FirstName = "Avery",
            LastName = "Worker",
            SSNLast4 = "1234",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2024, 1, 1),
            EmploymentStatus = EmploymentStatus.Active,
            WorkerType = WorkerType.W2Employee,
            PayType = PayType.Hourly,
            HourlyRate = 25m,
            ResidenceAddress = "100 Local Street",
            Address1 = "100 Local Street",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            FilingStatus = FederalFilingStatus.SingleOrMarriedFilingSeparately,
            W4Year = 2026,
            StateTaxState = "OK"
        };
    }

    [Fact]
    public void EmployeePayrollProfile_AllowsOnlySsnLastFour()
    {
        var employee = CreateValidHourlyEmployee();
        employee.FullSSNEncryptedPlaceholder = "123-45-6789";

        var results = Validate(employee);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(Employee.FullSSNEncryptedPlaceholder)));
    }

    [Fact]
    public void EmployeePayrollProfile_RequiresHourlyRateForHourlyEmployee()
    {
        var employee = CreateValidHourlyEmployee();
        employee.HourlyRate = null;

        var results = Validate(employee);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(Employee.HourlyRate)));
    }

    [Fact]
    public void EmployeePayrollProfile_RequiresAnnualSalaryForSalaryEmployee()
    {
        var employee = CreateValidHourlyEmployee();
        employee.PayType = PayType.Salary;
        employee.HourlyRate = null;
        employee.AnnualSalary = null;

        var results = Validate(employee);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(Employee.AnnualSalary)));
    }

    [Fact]
    public void EmployeePayrollProfile_RejectsInvalidW4Year()
    {
        var employee = CreateValidHourlyEmployee();
        employee.W4Year = 2019;

        var results = Validate(employee);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(Employee.W4Year)));
    }

    [Fact]
    public void EmployeeBankAccount_RejectsPlainTextRoutingAndAccountNumbers()
    {
        var account = CreateValidAccount();
        account.RoutingNumberToken = "123456789";
        account.AccountNumberToken = "1234567890";

        var results = Validate(account);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(EmployeeBankAccount.RoutingNumberToken)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(EmployeeBankAccount.AccountNumberToken)));
    }

    [Fact]
    public void EmployeeBankAccount_RequiresFixedDepositAmount()
    {
        var account = CreateValidAccount();
        account.DepositType = DirectDepositDepositType.FixedAmount;
        account.IsRemainderAccount = false;
        account.DepositAmount = null;

        var results = Validate(account);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(EmployeeBankAccount.DepositAmount)));
    }

    [Fact]
    public void EmployeeBankAccount_RequiresValidPercentage()
    {
        var account = CreateValidAccount();
        account.DepositType = DirectDepositDepositType.Percentage;
        account.IsRemainderAccount = false;
        account.DepositPercent = 101m;

        var results = Validate(account);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(EmployeeBankAccount.DepositPercent)));
    }

    [Fact]
    public void DirectDepositProfile_AllowsNoMoreThanFiveActiveAccounts()
    {
        var accounts = Enumerable.Range(1, 6)
            .Select(index =>
            {
                var account = CreateValidAccount();
                account.PriorityOrder = index <= 5 ? index : 5;
                account.DepositType = DirectDepositDepositType.FixedAmount;
                account.IsRemainderAccount = false;
                account.DepositAmount = 10m;
                return account;
            });

        var results = EmployeePayrollProfileValidator.ValidateDirectDepositAccounts(accounts);

        Assert.Contains(results, result => result.ErrorMessage?.Contains("no more than 5") == true);
    }

    private static EmployeeBankAccount CreateValidAccount()
    {
        return new EmployeeBankAccount
        {
            EmployeeId = 1,
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
}
