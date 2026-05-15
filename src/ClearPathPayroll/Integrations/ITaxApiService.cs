namespace ClearPathPayroll.Integrations;

public interface ITaxApiService
{
    // Placeholder for tax calculation API
    Task<decimal> CalculateTaxesAsync(decimal grossPay, string state);
}