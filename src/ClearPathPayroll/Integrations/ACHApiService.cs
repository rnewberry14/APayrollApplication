using ClearPathPayroll.Integrations;

namespace ClearPathPayroll.Integrations;

public class ACHApiService : IACHApiService
{
    public Task<bool> SubmitDirectDepositAsync(string employeeId, decimal amount, string accountDetails)
    {
        // Placeholder implementation - always succeed
        return Task.FromResult(true);
    }
}