namespace ClearPathPayroll.Integrations;

/// <summary>
/// Address information for tax calculation.
/// </summary>
public class AddressInfo
{
    /// <summary>
    /// Street address.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// City.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State (two-letter abbreviation).
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// ZIP code.
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// County.
    /// </summary>
    public string? County { get; set; }
}

/// <summary>
/// Employee tax request details.
/// </summary>
public class EmployeeTaxRequest
{
    /// <summary>
    /// Employee ID (for reference).
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// Gross wages for the pay period.
    /// </summary>
    public decimal GrossWages { get; set; }

    /// <summary>
    /// Taxable wages (after pre-tax deductions like 401k).
    /// </summary>
    public decimal TaxableWages { get; set; }

    /// <summary>
    /// Filing status (Single, Married, etc.).
    /// </summary>
    public string FilingStatus { get; set; } = "Single";

    /// <summary>
    /// Number of withholding allowances/exemptions.
    /// </summary>
    public int WithholdingAllowances { get; set; } = 1;

    /// <summary>
    /// Employee's residence address (for state/local taxes).
    /// </summary>
    public AddressInfo? ResidenceAddress { get; set; }

    /// <summary>
    /// Employee's work address (may differ from residence).
    /// </summary>
    public AddressInfo? WorkAddress { get; set; }

    /// <summary>
    /// Whether to subject to Social Security tax.
    /// </summary>
    public bool SubjectToSocialSecurityTax { get; set; } = true;

    /// <summary>
    /// Whether to subject to Medicare tax.
    /// </summary>
    public bool SubjectToMedicareTax { get; set; } = true;

    /// <summary>
    /// Whether to subject to federal income tax withholding.
    /// </summary>
    public bool SubjectToFederalWithholding { get; set; } = true;

    /// <summary>
    /// Whether to subject to state income tax withholding.
    /// </summary>
    public bool SubjectToStateWithholding { get; set; } = true;

    /// <summary>
    /// Whether to subject to local income tax withholding.
    /// </summary>
    public bool SubjectToLocalWithholding { get; set; } = true;
}

/// <summary>
/// Tax calculation request containing employee and payroll information.
/// </summary>
public class TaxCalculationRequest
{
    /// <summary>
    /// Company ID (for API routing/reference).
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Pay period start date.
    /// </summary>
    public DateTime PayPeriodStart { get; set; }

    /// <summary>
    /// Pay period end date.
    /// </summary>
    public DateTime PayPeriodEnd { get; set; }

    /// <summary>
    /// Actual pay date.
    /// </summary>
    public DateTime PayDate { get; set; }

    /// <summary>
    /// Tax year (for lookups/validation).
    /// </summary>
    public int TaxYear { get; set; } = DateTime.UtcNow.Year;

    /// <summary>
    /// Pay frequency (Weekly, Biweekly, Semimonthly, Monthly).
    /// </summary>
    public string PayFrequency { get; set; } = "Biweekly";

    /// <summary>
    /// Employee tax request details.
    /// </summary>
    public EmployeeTaxRequest Employee { get; set; } = new();

    /// <summary>
    /// Optional external reference (e.g., payroll vendor ID).
    /// </summary>
    public string? ExternalReference { get; set; }

    /// <summary>
    /// API version to target (for versioning).
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";
}
