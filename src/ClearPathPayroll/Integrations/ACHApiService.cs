using ClearPathPayroll.Configuration;
using Microsoft.Extensions.Options;

namespace ClearPathPayroll.Integrations;

public class ACHApiService : IACHApiService
{
    private readonly AchApiOptions _options;

    public ACHApiService(IOptions<AchApiOptions> options)
    {
        _options = options.Value;
    }

    public Task<bool> SubmitDirectDepositAsync(string employeeId, decimal amount, string accountDetails)
    {
        // Placeholder implementation - always succeed.
        // External ACH settings are loaded through options for future implementation.
        var apiUrl = _options.BaseUrl;
        var apiKey = _options.ApiKey;

        return Task.FromResult(true);
    }
}