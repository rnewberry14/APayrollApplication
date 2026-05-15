using ClearPathPayroll.Integrations;

namespace ClearPathPayroll.Integrations;

/// <summary>
/// Fake tax calculation service for testing/development.
/// Uses simple placeholder tax rates and returns detailed tax line items.
/// </summary>
public class FakeTaxCalculationService : ITaxCalculationService
{
    /// <summary>
    /// Legacy method - calculates taxes based on gross pay and state.
    /// Maintained for backward compatibility.
    /// </summary>
    public Task<TaxCalculationResult> CalculateTaxesAsync(decimal grossPay, string state, string employeeState = "OK")
    {
        // Placeholder rates for development/testing
        var socialSecurityRate = 0.062m; // 6.2%
        var medicareRate = 0.0145m; // 1.45%
        
        var socialSecurityTax = Math.Round(grossPay * socialSecurityRate, 2);
        var medicareTax = Math.Round(grossPay * medicareRate, 2);
        var federalIncomeTax = Math.Round(grossPay * 0.12m, 2); // Simplified 12% placeholder
        var stateIncomeTax = Math.Round(grossPay * 0.05m, 2); // Simplified 5% placeholder

        var totalEmployeeTaxes = Math.Round(federalIncomeTax + stateIncomeTax + socialSecurityTax + medicareTax, 2);
        
        // Employer taxes
        var employerSocialSecurityTax = socialSecurityTax;
        var employerMedicareTax = Math.Round(grossPay * 0.029m, 2); // 2.9%
        var totalEmployerTaxes = Math.Round(employerSocialSecurityTax + employerMedicareTax, 2);

        var result = new TaxCalculationResult
        {
            FederalIncomeTax = federalIncomeTax,
            StateIncomeTax = stateIncomeTax,
            SocialSecurityTax = socialSecurityTax,
            MedicareTax = medicareTax,
            TotalEmployeeTaxes = totalEmployeeTaxes,
            EmployerSocialSecurityTax = employerSocialSecurityTax,
            EmployerMedicareTax = employerMedicareTax,
            TotalEmployerTaxes = totalEmployerTaxes
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Comprehensive tax calculation based on detailed request.
    /// Returns line-item tax breakdown with external API reference for audit trail.
    /// </summary>
    public Task<TaxCalculationResponse> CalculateTaxesAsync(TaxCalculationRequest request)
    {
        var response = new TaxCalculationResponse
        {
            ApiVersion = request.ApiVersion,
            IsSuccessful = true,
            ExternalApiReference = GenerateApiReference(request),
            ResponseTimestamp = DateTime.UtcNow
        };

        var employee = request.Employee;

        // Employee taxes

        // Federal Income Tax (simplified - 12% of taxable wages)
        if (employee.SubjectToFederalWithholding)
        {
            var federalRate = 0.12m;
            var federalTax = Math.Round(employee.TaxableWages * federalRate, 2);
            response.EmployeeTaxLines.Add(new TaxLineResult
            {
                TaxType = "FederalIncomeTax",
                Description = "Federal Income Tax Withholding",
                Amount = federalTax,
                Rate = federalRate,
                Basis = employee.TaxableWages,
                Notes = $"Filing Status: {employee.FilingStatus}, Allowances: {employee.WithholdingAllowances}"
            });
        }

        // State Income Tax (simplified - 5% of taxable wages, uses residence state)
        if (employee.SubjectToStateWithholding && !string.IsNullOrEmpty(employee.ResidenceAddress?.State))
        {
            var stateRate = 0.05m;
            var stateTax = Math.Round(employee.TaxableWages * stateRate, 2);
            response.EmployeeTaxLines.Add(new TaxLineResult
            {
                TaxType = "StateIncomeTax",
                Description = $"{employee.ResidenceAddress.State} State Income Tax Withholding",
                Amount = stateTax,
                Rate = stateRate,
                Basis = employee.TaxableWages,
                Notes = $"Residence State: {employee.ResidenceAddress.State}"
            });
        }

        // Local Income Tax (placeholder - 2% if applicable, uses work address)
        if (employee.SubjectToLocalWithholding && !string.IsNullOrEmpty(employee.WorkAddress?.City))
        {
            var localRate = 0.02m;
            var localTax = Math.Round(employee.TaxableWages * localRate, 2);
            response.EmployeeTaxLines.Add(new TaxLineResult
            {
                TaxType = "LocalIncomeTax",
                Description = $"{employee.WorkAddress.City} Local Income Tax Withholding",
                Amount = localTax,
                Rate = localRate,
                Basis = employee.TaxableWages,
                Notes = $"Work Location: {employee.WorkAddress.City}, {employee.WorkAddress.State}"
            });
        }

        // Social Security Tax (6.2% of gross wages, subject to wage base)
        if (employee.SubjectToSocialSecurityTax)
        {
            var socialSecurityRate = 0.062m;
            var socialSecurityTax = Math.Round(employee.GrossWages * socialSecurityRate, 2);
            response.EmployeeTaxLines.Add(new TaxLineResult
            {
                TaxType = "SocialSecurityTax",
                Description = "Social Security Tax (FICA)",
                Amount = socialSecurityTax,
                Rate = socialSecurityRate,
                Basis = employee.GrossWages,
                Notes = "Subject to annual wage base limit ($168,600 in 2024)"
            });
        }

        // Medicare Tax (1.45% of gross wages)
        if (employee.SubjectToMedicareTax)
        {
            var medicareRate = 0.0145m;
            var medicareTax = Math.Round(employee.GrossWages * medicareRate, 2);
            response.EmployeeTaxLines.Add(new TaxLineResult
            {
                TaxType = "MedicareTax",
                Description = "Medicare Tax (FICA)",
                Amount = medicareTax,
                Rate = medicareRate,
                Basis = employee.GrossWages,
                Notes = "Additional Medicare Tax (0.9%) applies if wages exceed $200,000"
            });
        }

        // Employer taxes

        // Employer Social Security Tax (6.2% of gross wages)
        if (employee.SubjectToSocialSecurityTax)
        {
            var employerSsRate = 0.062m;
            var employerSsTax = Math.Round(employee.GrossWages * employerSsRate, 2);
            response.EmployerTaxLines.Add(new EmployerTaxLineResult
            {
                TaxType = "EmployerSocialSecurityTax",
                Description = "Employer Social Security Tax",
                Amount = employerSsTax,
                Rate = employerSsRate,
                Basis = employee.GrossWages,
                Notes = "Matching contribution"
            });
        }

        // Employer Medicare Tax (2.9% of gross wages)
        if (employee.SubjectToMedicareTax)
        {
            var employerMedicareRate = 0.029m;
            var employerMedicareTax = Math.Round(employee.GrossWages * employerMedicareRate, 2);
            response.EmployerTaxLines.Add(new EmployerTaxLineResult
            {
                TaxType = "EmployerMedicareTax",
                Description = "Employer Medicare Tax",
                Amount = employerMedicareTax,
                Rate = employerMedicareRate,
                Basis = employee.GrossWages,
                Notes = "Matching contribution"
            });
        }

        return Task.FromResult(response);
    }

    /// <summary>
    /// Generates a fake external API reference for audit trail.
    /// </summary>
    private string GenerateApiReference(TaxCalculationRequest request)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"FAKE-{request.CompanyId}-{request.Employee.EmployeeId}-{timestamp}";
    }
}