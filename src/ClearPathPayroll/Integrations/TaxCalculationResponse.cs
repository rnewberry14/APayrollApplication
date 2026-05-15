namespace ClearPathPayroll.Integrations;

/// <summary>
/// Individual tax line item result.
/// </summary>
public class TaxLineResult
{
    /// <summary>
    /// Tax type (FederalIncomeTax, StateIncomeTax, LocalIncomeTax, SocialSecurityTax, MedicareTax, etc.).
    /// </summary>
    public string TaxType { get; set; } = string.Empty;

    /// <summary>
    /// Tax description for display.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Tax amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Tax rate applied (e.g., 0.062 for 6.2% Social Security).
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Basis used for calculation (gross wages or taxable wages).
    /// </summary>
    public decimal Basis { get; set; }

    /// <summary>
    /// Optional notes about the calculation.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Employer tax line item result.
/// </summary>
public class EmployerTaxLineResult
{
    /// <summary>
    /// Tax type (EmployerSocialSecurityTax, EmployerMedicareTax, EmployerStateUnemployment, etc.).
    /// </summary>
    public string TaxType { get; set; } = string.Empty;

    /// <summary>
    /// Tax description for display.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Tax amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Tax rate applied.
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Basis used for calculation (gross wages or taxable wages).
    /// </summary>
    public decimal Basis { get; set; }

    /// <summary>
    /// Optional notes about the calculation.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Tax calculation response containing all calculated taxes.
/// </summary>
public class TaxCalculationResponse
{
    /// <summary>
    /// External API response reference (for audit trail).
    /// </summary>
    public string? ExternalApiReference { get; set; }

    /// <summary>
    /// Response timestamp.
    /// </summary>
    public DateTime ResponseTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tax API version used for calculation.
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// Whether the response indicates success.
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Optional error message if calculation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Individual employee tax lines.
    /// </summary>
    public List<TaxLineResult> EmployeeTaxLines { get; set; } = new();

    /// <summary>
    /// Employer tax lines.
    /// </summary>
    public List<EmployerTaxLineResult> EmployerTaxLines { get; set; } = new();

    // Convenience properties for total calculations

    /// <summary>
    /// Total federal income tax.
    /// </summary>
    public decimal FederalIncomeTax => EmployeeTaxLines
        .Where(t => t.TaxType == "FederalIncomeTax")
        .Sum(t => t.Amount);

    /// <summary>
    /// Total state income tax.
    /// </summary>
    public decimal StateIncomeTax => EmployeeTaxLines
        .Where(t => t.TaxType == "StateIncomeTax")
        .Sum(t => t.Amount);

    /// <summary>
    /// Total local income tax.
    /// </summary>
    public decimal LocalIncomeTax => EmployeeTaxLines
        .Where(t => t.TaxType == "LocalIncomeTax")
        .Sum(t => t.Amount);

    /// <summary>
    /// Total Social Security tax (employee).
    /// </summary>
    public decimal SocialSecurityTax => EmployeeTaxLines
        .Where(t => t.TaxType == "SocialSecurityTax")
        .Sum(t => t.Amount);

    /// <summary>
    /// Total Medicare tax (employee).
    /// </summary>
    public decimal MedicareTax => EmployeeTaxLines
        .Where(t => t.TaxType == "MedicareTax")
        .Sum(t => t.Amount);

    /// <summary>
    /// Total employee taxes.
    /// </summary>
    public decimal TotalEmployeeTaxes => EmployeeTaxLines.Sum(t => t.Amount);

    /// <summary>
    /// Total employer Social Security tax.
    /// </summary>
    public decimal EmployerSocialSecurityTax => EmployerTaxLines
        .Where(t => t.TaxType == "EmployerSocialSecurityTax")
        .Sum(t => t.Amount);

    /// <summary>
    /// Total employer Medicare tax.
    /// </summary>
    public decimal EmployerMedicareTax => EmployerTaxLines
        .Where(t => t.TaxType == "EmployerMedicareTax")
        .Sum(t => t.Amount);

    /// <summary>
    /// Total employer taxes.
    /// </summary>
    public decimal TotalEmployerTaxes => EmployerTaxLines.Sum(t => t.Amount);
}
