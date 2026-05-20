using System.Threading.Tasks;
using ClearPathPayroll.Configuration;
using ClearPathPayroll.Integrations;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollTaxApiIntegrationTests
{
    [Fact(Skip = "Integration placeholder for PayrollTaxAPI sandbox environment - requires valid sandbox BaseUrl and ApiKey.")]
    public async Task CalculateTaxesAsync_SandboxIntegrationPlaceholder()
    {
        var options = Options.Create(new TaxApiOptions
        {
            BaseUrl = "https://sandbox.payrolltaxapi.com/",
            ApiKey = "ENTER_REAL_SANDBOX_API_KEY",
            SandboxMode = true
        });

        var logger = NullLogger<PayrollTaxApiCalculationService>.Instance;

        using var httpClient = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(options.Value.BaseUrl) };
        var service = new PayrollTaxApiCalculationService(httpClient, options, logger);

        var request = new TaxCalculationRequest
        {
            CompanyId = 100,
            PayPeriodStart = System.DateTime.UtcNow.Date.AddDays(-14),
            PayPeriodEnd = System.DateTime.UtcNow.Date,
            PayDate = System.DateTime.UtcNow.Date.AddDays(1),
            TaxYear = System.DateTime.UtcNow.Year,
            PayFrequency = "Biweekly",
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 1000,
                GrossWages = 1500m,
                TaxableWages = 1500m,
                FilingStatus = "Single",
                ResidenceAddress = new AddressInfo { State = "OK" },
                WorkAddress = new AddressInfo { City = "Oklahoma City", State = "OK" }
            }
        };

        var response = await service.CalculateTaxesAsync(request);

        Assert.NotNull(response);
    }
}
