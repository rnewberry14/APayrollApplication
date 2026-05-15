using ClearPathPayroll.Integrations;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for the comprehensive tax calculation API abstraction.
/// </summary>
public class TaxCalculationServiceTests
{
    private readonly FakeTaxCalculationService _service = new();

    #region Legacy Method Tests

    [Fact]
    public async Task CalculateTaxes_LegacyMethod_ReturnsExpectedTotals()
    {
        var grossPay = 2000m;
        var state = "OK";

        var result = await _service.CalculateTaxesAsync(grossPay, state);

        Assert.Equal(Math.Round(2000m * 0.12m, 2), result.FederalIncomeTax);
        Assert.Equal(Math.Round(2000m * 0.05m, 2), result.StateIncomeTax);
        Assert.Equal(Math.Round(2000m * 0.062m, 2), result.SocialSecurityTax);
        Assert.Equal(Math.Round(2000m * 0.0145m, 2), result.MedicareTax);
        Assert.True(result.TotalEmployeeTaxes > 0);
        Assert.True(result.TotalEmployerTaxes > 0);
    }

    [Fact]
    public async Task CalculateTaxes_LegacyMethod_WithDifferentGrossAmounts()
    {
        var testCases = new[] { 1000m, 2500m, 5000m };

        foreach (var grossPay in testCases)
        {
            var result = await _service.CalculateTaxesAsync(grossPay, "OK");

            Assert.Equal(Math.Round(grossPay * 0.12m, 2), result.FederalIncomeTax);
            Assert.Equal(Math.Round(grossPay * 0.062m, 2), result.SocialSecurityTax);
        }
    }

    #endregion

    #region Comprehensive Request/Response Tests

    [Fact]
    public async Task CalculateTaxes_ComprehensiveMethod_ReturnsSuccessfulResponse()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessful);
        Assert.NotNull(response.ExternalApiReference);
        Assert.NotEmpty(response.ExternalApiReference);
        Assert.True(response.EmployeeTaxLines.Count > 0);
        Assert.True(response.EmployerTaxLines.Count > 0);
    }

    [Fact]
    public async Task CalculateTaxes_EmployeeTaxLines_ContainsAllTaxTypes()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);

        var taxTypes = response.EmployeeTaxLines.Select(t => t.TaxType).ToList();

        Assert.Contains("FederalIncomeTax", taxTypes);
        Assert.Contains("StateIncomeTax", taxTypes);
        Assert.Contains("SocialSecurityTax", taxTypes);
        Assert.Contains("MedicareTax", taxTypes);
    }

    [Fact]
    public async Task CalculateTaxes_EmployerTaxLines_ContainsMatchingTaxes()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);

        var employerTaxTypes = response.EmployerTaxLines.Select(t => t.TaxType).ToList();

        Assert.Contains("EmployerSocialSecurityTax", employerTaxTypes);
        Assert.Contains("EmployerMedicareTax", employerTaxTypes);
    }

    [Fact]
    public async Task CalculateTaxes_TaxLineResult_ContainsCompleteDetails()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);
        var fedTaxLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "FederalIncomeTax");

        Assert.NotNull(fedTaxLine);
        Assert.NotEmpty(fedTaxLine.Description);
        Assert.True(fedTaxLine.Amount > 0);
        Assert.Equal(0.12m, fedTaxLine.Rate);
        Assert.Equal(request.Employee.TaxableWages, fedTaxLine.Basis);
        Assert.NotEmpty(fedTaxLine.Notes);
    }

    [Fact]
    public async Task CalculateTaxes_ExternalApiReference_IncludesCompanyAndEmployeeIds()
    {
        var request = new TaxCalculationRequest
        {
            CompanyId = 42,
            Employee = new EmployeeTaxRequest { EmployeeId = 99 }
        };

        var response = await _service.CalculateTaxesAsync(request);

        Assert.Contains("42", response.ExternalApiReference);
        Assert.Contains("99", response.ExternalApiReference);
    }

    [Fact]
    public async Task CalculateTaxes_ConvenienceProperties_AggregateCorrectly()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);

        var expectedFederal = response.EmployeeTaxLines
            .Where(t => t.TaxType == "FederalIncomeTax")
            .Sum(t => t.Amount);
        Assert.Equal(expectedFederal, response.FederalIncomeTax);

        var expectedTotal = response.EmployeeTaxLines.Sum(t => t.Amount);
        Assert.Equal(expectedTotal, response.TotalEmployeeTaxes);

        var expectedEmployerTotal = response.EmployerTaxLines.Sum(t => t.Amount);
        Assert.Equal(expectedEmployerTotal, response.TotalEmployerTaxes);
    }

    #endregion

    #region Filing Status Tests

    [Fact]
    public async Task CalculateTaxes_FilingStatus_IsIncludedInNotes()
    {
        var request = CreateBasicRequest();
        request.Employee.FilingStatus = "Married";

        var response = await _service.CalculateTaxesAsync(request);

        var fedTaxLine = response.EmployeeTaxLines.First(t => t.TaxType == "FederalIncomeTax");
        Assert.Contains("Married", fedTaxLine.Notes);
    }

    [Fact]
    public async Task CalculateTaxes_WithholdingAllowances_AreTracked()
    {
        var request = CreateBasicRequest();
        request.Employee.WithholdingAllowances = 3;

        var response = await _service.CalculateTaxesAsync(request);

        var fedTaxLine = response.EmployeeTaxLines.First(t => t.TaxType == "FederalIncomeTax");
        Assert.Contains("3", fedTaxLine.Notes);
    }

    #endregion

    #region Address Tests

    [Fact]
    public async Task CalculateTaxes_ResidenceAddress_UsedForStateTax()
    {
        var request = CreateBasicRequest();
        request.Employee.ResidenceAddress = new AddressInfo
        {
            Street = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101"
        };

        var response = await _service.CalculateTaxesAsync(request);

        var stateTaxLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "StateIncomeTax");
        Assert.NotNull(stateTaxLine);
        Assert.Contains("OK", stateTaxLine.Description);
    }

    [Fact]
    public async Task CalculateTaxes_WorkAddress_UsedForLocalTax()
    {
        var request = CreateBasicRequest();
        request.Employee.WorkAddress = new AddressInfo
        {
            Street = "456 Work Ave",
            City = "Tulsa",
            State = "OK",
            ZipCode = "74103",
            County = "Tulsa"
        };

        var response = await _service.CalculateTaxesAsync(request);

        var localTaxLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "LocalIncomeTax");
        Assert.NotNull(localTaxLine);
        Assert.Contains("Tulsa", localTaxLine.Description);
    }

    [Fact]
    public async Task CalculateTaxes_NoWorkAddress_SkipsLocalTax()
    {
        var request = CreateBasicRequest();
        request.Employee.WorkAddress = null;
        request.Employee.SubjectToLocalWithholding = true;

        var response = await _service.CalculateTaxesAsync(request);

        var localTaxLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "LocalIncomeTax");
        Assert.Null(localTaxLine);
    }

    #endregion

    #region Withholding Exemption Tests

    [Fact]
    public async Task CalculateTaxes_NoFederalWithholding_SkipsFedTax()
    {
        var request = CreateBasicRequest();
        request.Employee.SubjectToFederalWithholding = false;

        var response = await _service.CalculateTaxesAsync(request);

        var fedTaxLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "FederalIncomeTax");
        Assert.Null(fedTaxLine);
    }

    [Fact]
    public async Task CalculateTaxes_NoStateWithholding_SkipsStateTax()
    {
        var request = CreateBasicRequest();
        request.Employee.SubjectToStateWithholding = false;

        var response = await _service.CalculateTaxesAsync(request);

        var stateTaxLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "StateIncomeTax");
        Assert.Null(stateTaxLine);
    }

    [Fact]
    public async Task CalculateTaxes_NoSocialSecurityTax_SkipsSSTax()
    {
        var request = CreateBasicRequest();
        request.Employee.SubjectToSocialSecurityTax = false;

        var response = await _service.CalculateTaxesAsync(request);

        var ssTaxLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "SocialSecurityTax");
        Assert.Null(ssTaxLine);

        var employerSsTax = response.EmployerTaxLines.FirstOrDefault(t => t.TaxType == "EmployerSocialSecurityTax");
        Assert.Null(employerSsTax);
    }

    [Fact]
    public async Task CalculateTaxes_NoMedicareTax_SkipsMedicareTax()
    {
        var request = CreateBasicRequest();
        request.Employee.SubjectToMedicareTax = false;

        var response = await _service.CalculateTaxesAsync(request);

        var medicareLine = response.EmployeeTaxLines.FirstOrDefault(t => t.TaxType == "MedicareTax");
        Assert.Null(medicareLine);

        var employerMedicare = response.EmployerTaxLines.FirstOrDefault(t => t.TaxType == "EmployerMedicareTax");
        Assert.Null(employerMedicare);
    }

    #endregion

    #region Tax Rate and Calculation Tests

    [Fact]
    public async Task CalculateTaxes_TaxRates_AreAppliedCorrectly()
    {
        var request = CreateBasicRequest();
        request.Employee.GrossWages = 1000m;
        request.Employee.TaxableWages = 900m;

        var response = await _service.CalculateTaxesAsync(request);

        var ssTax = response.EmployeeTaxLines.First(t => t.TaxType == "SocialSecurityTax");
        Assert.Equal(Math.Round(1000m * 0.062m, 2), ssTax.Amount);

        var medicareTax = response.EmployeeTaxLines.First(t => t.TaxType == "MedicareTax");
        Assert.Equal(Math.Round(1000m * 0.0145m, 2), medicareTax.Amount);

        var fedTax = response.EmployeeTaxLines.First(t => t.TaxType == "FederalIncomeTax");
        Assert.Equal(Math.Round(900m * 0.12m, 2), fedTax.Amount);
    }

    [Fact]
    public async Task CalculateTaxes_TaxBasis_DifferentiatesGrossVsTaxable()
    {
        var request = CreateBasicRequest();
        request.Employee.GrossWages = 2000m;
        request.Employee.TaxableWages = 1800m; // 401k deduction

        var response = await _service.CalculateTaxesAsync(request);

        var ssTax = response.EmployeeTaxLines.First(t => t.TaxType == "SocialSecurityTax");
        Assert.Equal(2000m, ssTax.Basis); // SS uses gross

        var fedTax = response.EmployeeTaxLines.First(t => t.TaxType == "FederalIncomeTax");
        Assert.Equal(1800m, fedTax.Basis); // Fed income uses taxable
    }

    [Fact]
    public async Task CalculateTaxes_EmployerTaxBasis_UsesGrossWages()
    {
        var request = CreateBasicRequest();
        request.Employee.GrossWages = 2000m;

        var response = await _service.CalculateTaxesAsync(request);

        var employerSsTax = response.EmployerTaxLines.First(t => t.TaxType == "EmployerSocialSecurityTax");
        Assert.Equal(2000m, employerSsTax.Basis);
        Assert.Equal(0.062m, employerSsTax.Rate);
    }

    [Fact]
    public async Task CalculateTaxes_RoundingIsCorrect()
    {
        var request = new TaxCalculationRequest
        {
            CompanyId = 1,
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 1,
                GrossWages = 1234.56m,
                TaxableWages = 1234.56m,
                ResidenceAddress = new AddressInfo { State = "OK" },
                FilingStatus = "Single"
            }
        };

        var response = await _service.CalculateTaxesAsync(request);

        foreach (var line in response.EmployeeTaxLines)
        {
            Assert.Equal(line.Amount, Math.Round(line.Amount, 2));
        }

        foreach (var line in response.EmployerTaxLines)
        {
            Assert.Equal(line.Amount, Math.Round(line.Amount, 2));
        }
    }

    #endregion

    #region Aggregate Property Tests

    [Fact]
    public async Task CalculateTaxes_TotalEmployeeTaxes_EqualsLineItemSum()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);

        var manualSum = response.EmployeeTaxLines.Sum(t => t.Amount);
        Assert.Equal(manualSum, response.TotalEmployeeTaxes);
    }

    [Fact]
    public async Task CalculateTaxes_TotalEmployerTaxes_EqualsLineItemSum()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);

        var manualSum = response.EmployerTaxLines.Sum(t => t.Amount);
        Assert.Equal(manualSum, response.TotalEmployerTaxes);
    }

    [Fact]
    public async Task CalculateTaxes_IndividualTaxTypes_AggregateCorrectly()
    {
        var request = CreateBasicRequest();

        var response = await _service.CalculateTaxesAsync(request);

        var fedSum = response.EmployeeTaxLines
            .Where(t => t.TaxType == "FederalIncomeTax")
            .Sum(t => t.Amount);
        Assert.Equal(fedSum, response.FederalIncomeTax);

        var stateSum = response.EmployeeTaxLines
            .Where(t => t.TaxType == "StateIncomeTax")
            .Sum(t => t.Amount);
        Assert.Equal(stateSum, response.StateIncomeTax);

        var ssSum = response.EmployeeTaxLines
            .Where(t => t.TaxType == "SocialSecurityTax")
            .Sum(t => t.Amount);
        Assert.Equal(ssSum, response.SocialSecurityTax);

        var medicareSum = response.EmployeeTaxLines
            .Where(t => t.TaxType == "MedicareTax")
            .Sum(t => t.Amount);
        Assert.Equal(medicareSum, response.MedicareTax);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task CalculateTaxes_ZeroGrossWages_CalculatesZeroTaxes()
    {
        var request = CreateBasicRequest();
        request.Employee.GrossWages = 0m;
        request.Employee.TaxableWages = 0m;

        var response = await _service.CalculateTaxesAsync(request);

        Assert.Equal(0m, response.TotalEmployeeTaxes);
        Assert.Equal(0m, response.TotalEmployerTaxes);
    }

    [Fact]
    public async Task CalculateTaxes_HighWages_CalculatesCorrectly()
    {
        var request = CreateBasicRequest();
        request.Employee.GrossWages = 10000m;
        request.Employee.TaxableWages = 10000m;

        var response = await _service.CalculateTaxesAsync(request);

        Assert.True(response.TotalEmployeeTaxes > 0);
        Assert.True(response.TotalEmployerTaxes > 0);
    }

    [Fact]
    public async Task CalculateTaxes_DefaultValues_WorkWithoutAddress()
    {
        var request = new TaxCalculationRequest
        {
            CompanyId = 1,
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 1,
                GrossWages = 2000m,
                TaxableWages = 2000m
            }
        };

        var response = await _service.CalculateTaxesAsync(request);

        Assert.True(response.IsSuccessful);
        Assert.True(response.EmployeeTaxLines.Count >= 2); // At least FICA taxes
    }

    #endregion

    #region Helper Methods

    private TaxCalculationRequest CreateBasicRequest()
    {
        return new TaxCalculationRequest
        {
            CompanyId = 1,
            PayPeriodStart = DateTime.UtcNow.Date.AddDays(-13),
            PayPeriodEnd = DateTime.UtcNow.Date,
            PayDate = DateTime.UtcNow.Date.AddDays(3),
            TaxYear = DateTime.UtcNow.Year,
            PayFrequency = "Biweekly",
            Employee = new EmployeeTaxRequest
            {
                EmployeeId = 1,
                GrossWages = 2000m,
                TaxableWages = 2000m,
                FilingStatus = "Single",
                WithholdingAllowances = 1,
                ResidenceAddress = new AddressInfo
                {
                    Street = "123 Main St",
                    City = "Oklahoma City",
                    State = "OK",
                    ZipCode = "73101"
                },
                WorkAddress = new AddressInfo
                {
                    Street = "456 Work Ave",
                    City = "Oklahoma City",
                    State = "OK",
                    ZipCode = "73102"
                },
                SubjectToSocialSecurityTax = true,
                SubjectToMedicareTax = true,
                SubjectToFederalWithholding = true,
                SubjectToStateWithholding = true,
                SubjectToLocalWithholding = true
            }
        };
    }

    #endregion
}
