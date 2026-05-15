namespace ClearPathPayroll.Integrations;

public interface IACHApiService
{
    // Placeholder for ACH/direct deposit API
    Task<bool> SubmitDirectDepositAsync(string employeeId, decimal amount, string accountDetails);
}