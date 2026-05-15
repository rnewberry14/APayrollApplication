namespace ClearPathPayroll.Integrations;

/// <summary>
/// Tax calculation result (legacy - for backward compatibility).
/// </summary>
public class TaxCalculationResult
{
    /// <summary>
    /// Federal income tax.
    /// </summary>
    public decimal FederalIncomeTax { get; set; }

    /// <summary>
    /// State income tax.
    /// </summary>
    public decimal StateIncomeTax { get; set; }

    /// <summary>
    /// Social Security tax (FICA).
    /// </summary>
    public decimal SocialSecurityTax { get; set; }

    /// <summary>
    /// Medicare tax (FICA).
    /// </summary>
    public decimal MedicareTax { get; set; }

    /// <summary>
    /// Total employee taxes.
    /// </summary>
    public decimal TotalEmployeeTaxes { get; set; }

    /// <summary>
    /// Employer Social Security tax.
    /// </summary>
    public decimal EmployerSocialSecurityTax { get; set; }

    /// <summary>
    /// Employer Medicare tax.
    /// </summary>
    public decimal EmployerMedicareTax { get; set; }

    /// <summary>
    /// Total employer taxes.
    /// </summary>
    public decimal TotalEmployerTaxes { get; set; }
}

/// <summary>
/// Interface for tax calculation service.
/// Supports both legacy and comprehensive tax calculation workflows.
/// </summary>
public interface ITaxCalculationService
{
    /// <summary>
    /// Calculates taxes for an employee based on gross pay and state (legacy method).
    /// </summary>
    [Obsolete("Use CalculateTaxesAsync(TaxCalculationRequest) instead.")]
    Task<TaxCalculationResult> CalculateTaxesAsync(decimal grossPay, string state, string employeeState = "OK");

    /// <summary>
    /// Calculates comprehensive taxes based on detailed tax request.
    /// Supports federal, state, local taxes, and employer taxes with full audit trail.
    /// </summary>
    Task<TaxCalculationResponse> CalculateTaxesAsync(TaxCalculationRequest request);
}