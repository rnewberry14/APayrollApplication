using ClearPathPayroll.Integrations;

namespace ClearPathPayroll.Integrations;

public class TaxApiService : ITaxApiService
{
    public Task<decimal> CalculateTaxesAsync(decimal grossPay, string state)
    {
        // Placeholder implementation - return 20% tax
        return Task.FromResult(grossPay * 0.20m);
    }
}