using ClearPathPayroll.Configuration;
using Microsoft.Extensions.Options;

namespace ClearPathPayroll.Integrations;

public class TaxApiService : ITaxApiService
{
    private readonly TaxApiOptions _options;

    public TaxApiService(IOptions<TaxApiOptions> options)
    {
        _options = options.Value;
    }

    public Task<decimal> CalculateTaxesAsync(decimal grossPay, string state)
    {
        // Placeholder implementation - return 20% tax.
        // External API settings are loaded through options for future implementation.
        var apiUrl = _options.BaseUrl;
        var apiKey = _options.ApiKey;

        return Task.FromResult(grossPay * 0.20m);
    }
}