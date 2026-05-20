using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClearPathPayroll.Configuration;
using ClearPathPayroll.Integrations;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PayrollTaxApiCalculationServiceTests
{
    [Fact]
    public async Task CalculateTaxesAsync_ParsesVendorResponseIntoTaxCalculationResponse()
    {
        var vendorResponse = new
        {
            referenceId = "TX12345",
            version = "1.1",
            success = true,
            federalRate = 0.12m,
            stateRate = 0.05m,
            localRate = 0.02m,
            socialSecurityRate = 0.062m,
            medicareRate = 0.0145m,
            employerSocialSecurityRate = 0.062m,
            employerMedicareRate = 0.029m
        };

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(vendorResponse));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.tax.example.com/") };
        var options = Options.Create(new TaxApiOptions { BaseUrl = "https://api.tax.example.com", ApiKey = "test-key", SandboxMode = true });
        var logger = NullLogger<PayrollTaxApiCalculationService>.Instance;

        var service = new PayrollTaxApiCalculationService(client, options, logger);

        var request = new TaxCalculationRequest
        {
            CompanyId = 1,
            PayPeriodStart = DateTime.UtcNow.Date.AddDays(-14),
            PayPeriodEnd = DateTime.UtcNow.Date,
            PayDate = DateTime.UtcNow.Date.AddDays(1),
            TaxYear = DateTime.UtcNow.Year,
            PayFrequency = "Biweekly",
            ApiVersion = "1.0",
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 99,
                GrossWages = 1000m,
                TaxableWages = 1000m,
                FilingStatus = "Single",
                ResidenceAddress = new AddressInfo { State = "OK" },
                WorkAddress = new AddressInfo { City = "Tulsa", State = "OK" }
            }
        };

        var result = await service.CalculateTaxesAsync(request);

        Assert.True(result.IsSuccessful);
        Assert.Equal("TX12345", result.ExternalApiReference);
        Assert.Equal("1.1", result.ApiVersion);
        Assert.Equal(5, result.EmployeeTaxLines.Count);
        Assert.Equal(2, result.EmployerTaxLines.Count);
        Assert.Equal(120m, result.FederalIncomeTax);
        Assert.Equal(62m, result.EmployerSocialSecurityTax);
    }

    [Fact]
    public async Task CalculateTaxesAsync_ReturnsErrorResponseWhenHttpFails()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.InternalServerError, "{ \"error\": \"server failure\" }");
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.tax.example.com/") };
        var options = Options.Create(new TaxApiOptions { BaseUrl = "https://api.tax.example.com", ApiKey = "test-key", SandboxMode = false });
        var logger = NullLogger<PayrollTaxApiCalculationService>.Instance;
        var service = new PayrollTaxApiCalculationService(client, options, logger);

        var request = new TaxCalculationRequest
        {
            CompanyId = 2,
            PayPeriodStart = DateTime.UtcNow.Date.AddDays(-14),
            PayPeriodEnd = DateTime.UtcNow.Date,
            PayDate = DateTime.UtcNow.Date.AddDays(1),
            TaxYear = DateTime.UtcNow.Year,
            PayFrequency = "Biweekly",
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 10,
                GrossWages = 1000m,
                TaxableWages = 1000m,
                WorkAddress = new AddressInfo { City = "Tulsa", State = "OK" }
            }
        };

        var result = await service.CalculateTaxesAsync(request);

        Assert.False(result.IsSuccessful);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.EmployeeTaxLines);
        Assert.Empty(result.EmployerTaxLines);
    }

    [Fact]
    public async Task CalculateTaxesAsync_SetsSandboxHeaderWhenSandboxModeEnabled()
    {
        var vendorResponse = new
        {
            referenceId = "TX12345",
            version = "1.0",
            success = true,
            employeeTaxes = Array.Empty<object>(),
            employerTaxes = Array.Empty<object>()
        };

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(vendorResponse));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.tax.example.com/") };
        var options = Options.Create(new TaxApiOptions { BaseUrl = "https://api.tax.example.com", ApiKey = "test-key", SandboxMode = true });
        var logger = NullLogger<PayrollTaxApiCalculationService>.Instance;

        var service = new PayrollTaxApiCalculationService(client, options, logger);

        var request = new TaxCalculationRequest
        {
            CompanyId = 3,
            PayPeriodStart = DateTime.UtcNow.Date.AddDays(-14),
            PayPeriodEnd = DateTime.UtcNow.Date,
            PayDate = DateTime.UtcNow.Date.AddDays(1),
            TaxYear = DateTime.UtcNow.Year,
            PayFrequency = "Biweekly",
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 15,
                GrossWages = 1000m,
                TaxableWages = 1000m,
                WorkAddress = new AddressInfo { City = "Tulsa", State = "OK" }
            }
        };

        await service.CalculateTaxesAsync(request);

        Assert.Contains("X-Sandbox-Mode", handler.LastRequest.Headers.Select(header => header.Key));
        Assert.Equal("true", handler.LastRequest.Headers.GetValues("X-Sandbox-Mode").Single());
    }

    private sealed class TestHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public HttpRequestMessage LastRequest { get; private set; } = null!;

        public TestHttpMessageHandler(HttpStatusCode statusCode, string responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}
