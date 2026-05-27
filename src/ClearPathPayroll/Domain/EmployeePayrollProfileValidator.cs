using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public static class EmployeePayrollProfileValidator
{
    public static IReadOnlyList<ValidationResult> ValidateDirectDepositAccounts(IEnumerable<EmployeeBankAccount> accounts)
    {
        var activeAccounts = accounts.Where(account => account.IsActive).ToList();
        var results = new List<ValidationResult>();

        if (activeAccounts.Count > 5)
        {
            results.Add(new ValidationResult("An employee can have no more than 5 active direct deposit accounts."));
        }

        if (activeAccounts.Count(account => account.IsRemainderAccount || account.DepositType == DirectDepositDepositType.Remainder) > 1)
        {
            results.Add(new ValidationResult("An employee can have only one active remainder direct deposit account."));
        }

        var duplicatePriority = activeAccounts
            .GroupBy(account => account.PriorityOrder)
            .Any(group => group.Count() > 1);
        if (duplicatePriority)
        {
            results.Add(new ValidationResult("Active direct deposit account priority/order values must be unique."));
        }

        return results;
    }
}
