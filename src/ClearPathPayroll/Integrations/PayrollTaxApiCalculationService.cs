using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ClearPathPayroll.Configuration;

namespace ClearPathPayroll.Integrations;

/// <summary>
/// PayrollTaxAPI.com sandbox integration service.
/// Uses HttpClientFactory, configuration, and safe logging.
/// </summary>
public class PayrollTaxApiCalculationService : ITaxCalculationService
{
    private readonly HttpClient _httpClient;
    private readonly TaxApiOptions _options;
    private readonly ILogger<PayrollTaxApiCalculationService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public PayrollTaxApiCalculationService(
        HttpClient httpClient,
        IOptions<TaxApiOptions> options,
        ILogger<PayrollTaxApiCalculationService> logger)
    {
        _httpClient = httpClient;
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Obsolete("Use CalculateTaxesAsync(TaxCalculationRequest) instead.")]
    public async Task<TaxCalculationResult> CalculateTaxesAsync(decimal grossPay, string state, string employeeState = "OK")
    {
        var request = new TaxCalculationRequest
        {
            CompanyId = 0,
            PayPeriodStart = DateTime.UtcNow.Date.AddDays(-14),
            PayPeriodEnd = DateTime.UtcNow.Date,
            PayDate = DateTime.UtcNow.Date.AddDays(1),
            TaxYear = DateTime.UtcNow.Year,
            PayFrequency = "Biweekly",
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 0,
                GrossWages = grossPay,
                TaxableWages = grossPay,
                FilingStatus = "Single",
                WithholdingAllowances = 1,
                ResidenceAddress = new AddressInfo { State = state },
                WorkAddress = new AddressInfo { State = employeeState }
            }
        };

        var response = await CalculateTaxesAsync(request);

        return new TaxCalculationResult
        {
            FederalIncomeTax = response.FederalIncomeTax,
            StateIncomeTax = response.StateIncomeTax,
            SocialSecurityTax = response.SocialSecurityTax,
            MedicareTax = response.MedicareTax,
            TotalEmployeeTaxes = response.TotalEmployeeTaxes,
            EmployerSocialSecurityTax = response.EmployerSocialSecurityTax,
            EmployerMedicareTax = response.EmployerMedicareTax,
            TotalEmployerTaxes = response.TotalEmployerTaxes
        };
    }

    public async Task<TaxCalculationResponse> CalculateTaxesAsync(TaxCalculationRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.Employee is null)
        {
            throw new ArgumentException("Employee details are required for tax calculation.", nameof(request));
        }

        var workState = request.Employee.WorkAddress?.State ?? request.Employee.ResidenceAddress?.State;
        if (string.IsNullOrWhiteSpace(workState))
        {
            throw new InvalidOperationException("Work state or residence state is required for PayrollTaxAPI rate lookup.");
        }

        _logger.LogInformation("PayrollTaxAPI request: CompanyId={CompanyId}, EmployeeId={EmployeeId}, WorkState={WorkState}, PayDate={PayDate:yyyy-MM-dd}, Sandbox={Sandbox}",
            request.CompanyId,
            request.Employee.EmployeeId,
            workState,
            request.PayDate,
            _options.SandboxMode);

        var lookupUri = $"rates/lookup?workState={Uri.EscapeDataString(workState)}&payDate={request.PayDate:yyyy-MM-dd}";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, lookupUri);
        if (_options.SandboxMode && !httpRequest.Headers.Contains("X-Sandbox-Mode"))
        {
            httpRequest.Headers.Add("X-Sandbox-Mode", "true");
        }

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PayrollTaxAPI returned status {StatusCode} for CompanyId={CompanyId}, EmployeeId={EmployeeId}.",
                    response.StatusCode,
                    request.CompanyId,
                    request.Employee.EmployeeId);

                return new TaxCalculationResponse
                {
                    ApiVersion = request.ApiVersion,
                    ExternalApiReference = null,
                    ResponseTimestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = $"Tax lookup failed with status {response.StatusCode}."
                };
            }

            var ratesResponse = JsonSerializer.Deserialize<PayrollTaxApiRateResponse>(responseText, _jsonOptions);
            if (ratesResponse is null)
            {
                _logger.LogWarning("PayrollTaxAPI returned an empty or unexpected payload for CompanyId={CompanyId}, EmployeeId={EmployeeId}.",
                    request.CompanyId,
                    request.Employee.EmployeeId);

                return new TaxCalculationResponse
                {
                    ApiVersion = request.ApiVersion,
                    ExternalApiReference = null,
                    ResponseTimestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = "Tax lookup service returned an invalid response format."
                };
            }

            return MapRatesResponse(ratesResponse, request, workState);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "PayrollTaxAPI request failed for CompanyId={CompanyId}, EmployeeId={EmployeeId}.",
                request.CompanyId,
                request.Employee.EmployeeId);

            return new TaxCalculationResponse
            {
                ApiVersion = request.ApiVersion,
                ExternalApiReference = null,
                ResponseTimestamp = DateTime.UtcNow,
                IsSuccessful = false,
                ErrorMessage = "Unable to contact the tax calculation service."
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "PayrollTaxAPI response JSON parsing failed for CompanyId={CompanyId}, EmployeeId={EmployeeId}.",
                request.CompanyId,
                request.Employee.EmployeeId);

            return new TaxCalculationResponse
            {
                ApiVersion = request.ApiVersion,
                ExternalApiReference = null,
                ResponseTimestamp = DateTime.UtcNow,
                IsSuccessful = false,
                ErrorMessage = "Received malformed tax calculation response."
            };
        }
    }

    private TaxCalculationResponse MapRatesResponse(PayrollTaxApiRateResponse rates, TaxCalculationRequest request, string workState)
    {
        var response = new TaxCalculationResponse
        {
            ApiVersion = string.IsNullOrWhiteSpace(rates.Version) ? request.ApiVersion : rates.Version,
            ExternalApiReference = rates.ReferenceId,
            ResponseTimestamp = DateTime.UtcNow,
            IsSuccessful = rates.Success,
            ErrorMessage = rates.Success ? null : rates.Message
        };

        var employee = request.Employee;
        var taxableBasis = employee.TaxableWages;
        var grossBasis = employee.GrossWages;

        if (rates.Success)
        {
            if (employee.SubjectToFederalWithholding)
            {
                response.EmployeeTaxLines.Add(CreateTaxLine("FederalIncomeTax", "Federal Income Tax Withholding", taxableBasis, rates.FederalRate, workState));
            }

            if (employee.SubjectToStateWithholding)
            {
                response.EmployeeTaxLines.Add(CreateTaxLine("StateIncomeTax", $"{workState} State Income Tax Withholding", taxableBasis, rates.StateRate, workState));
            }

            if (employee.SubjectToLocalWithholding)
            {
                response.EmployeeTaxLines.Add(CreateTaxLine("LocalIncomeTax", "Local Income Tax Withholding", taxableBasis, rates.LocalRate, workState));
            }

            if (employee.SubjectToSocialSecurityTax)
            {
                response.EmployeeTaxLines.Add(CreateTaxLine("SocialSecurityTax", "Social Security Tax (FICA)", grossBasis, rates.SocialSecurityRate, workState));
                response.EmployerTaxLines.Add(CreateEmployerTaxLine("EmployerSocialSecurityTax", "Employer Social Security Tax", grossBasis, rates.EmployerSocialSecurityRate ?? rates.SocialSecurityRate, workState));
            }

            if (employee.SubjectToMedicareTax)
            {
                response.EmployeeTaxLines.Add(CreateTaxLine("MedicareTax", "Medicare Tax (FICA)", grossBasis, rates.MedicareRate, workState));
                response.EmployerTaxLines.Add(CreateEmployerTaxLine("EmployerMedicareTax", "Employer Medicare Tax", grossBasis, rates.EmployerMedicareRate ?? 0.029m, workState));
            }
        }

        return response;
    }

    private static TaxLineResult CreateTaxLine(string type, string description, decimal basis, decimal rate, string workState)
    {
        var amount = Math.Round(basis * rate, 2);
        return new TaxLineResult
        {
            TaxType = type,
            Description = description,
            Amount = amount,
            Rate = rate,
            Basis = basis,
            Notes = $"Work State: {workState}"
        };
    }

    private static EmployerTaxLineResult CreateEmployerTaxLine(string type, string description, decimal basis, decimal rate, string workState)
    {
        var amount = Math.Round(basis * rate, 2);
        return new EmployerTaxLineResult
        {
            TaxType = type,
            Description = description,
            Amount = amount,
            Rate = rate,
            Basis = basis,
            Notes = $"Work State: {workState}"
        };
    }

    private sealed class PayrollTaxApiRateResponse
    {
        public string ReferenceId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Message { get; set; }
        public decimal FederalRate { get; set; }
        public decimal StateRate { get; set; }
        public decimal LocalRate { get; set; }
        public decimal SocialSecurityRate { get; set; } = 0.062m;
        public decimal MedicareRate { get; set; } = 0.0145m;
        public decimal? EmployerSocialSecurityRate { get; set; }
        public decimal? EmployerMedicareRate { get; set; }
    }
}
