using ClearPathPayroll.Domain;

namespace ClearPathPayroll.Services;

/// <summary>
/// Service for calculating employee deductions.
/// </summary>
public class DeductionCalculationService
{
    /// <summary>
    /// Calculates deductions for an employee based on gross and net pay.
    /// </summary>
    /// <param name="grossPay">Gross pay amount.</param>
    /// <param name="netPayBeforeDeductions">Net pay before deductions are applied (after taxes would be applied).</param>
    /// <param name="deductions">List of deductions to apply.</param>
    /// <returns>Deduction calculation result with line items.</returns>
    public DeductionCalculationResult CalculateDeductions(decimal grossPay, decimal netPayBeforeDeductions, List<DeductionInput> deductions)
    {
        var result = new DeductionCalculationResult();

        if (deductions == null || deductions.Count == 0)
        {
            return result;
        }

        // Calculate pre-tax deductions first
        var preTaxDeductions = deductions
            .Where(d => d.Type == DeductionType.PreTaxFixedAmount || d.Type == DeductionType.PreTaxPercentage)
            .ToList();

        var preTaxAmount = 0m;
        foreach (var deduction in preTaxDeductions)
        {
            var amount = CalculateDeductionAmount(deduction, grossPay);
            amount = Math.Round(amount, 2);

            // Check if deduction would exceed gross pay
            string? adjustmentReason = null;
            if (preTaxAmount + amount > grossPay)
            {
                amount = grossPay - preTaxAmount;
                adjustmentReason = "Adjusted to not exceed gross pay";
                result.HasAdjustments = true;
            }

            if (amount > 0)
            {
                result.DeductionLines.Add(new DeductionResultLine
                {
                    Description = deduction.Description,
                    Type = deduction.Type,
                    Amount = amount,
                    AdjustmentReason = adjustmentReason
                });

                preTaxAmount += amount;
            }
        }

        result.TotalPreTaxDeductions = Math.Round(preTaxAmount, 2);

        // Calculate post-tax deductions
        var postTaxDeductions = deductions
            .Where(d => d.Type == DeductionType.PostTaxFixedAmount || d.Type == DeductionType.PostTaxPercentage)
            .ToList();

        var postTaxAmount = 0m;
        var availableForPostTax = netPayBeforeDeductions;

        foreach (var deduction in postTaxDeductions)
        {
            var originalAmount = CalculateDeductionAmount(deduction, grossPay);
            originalAmount = Math.Round(originalAmount, 2);
            var amount = originalAmount;

            // Prevent deductions from creating negative net pay
            string? adjustmentReason = null;
            if (postTaxAmount + amount > availableForPostTax)
            {
                amount = Math.Max(0, availableForPostTax - postTaxAmount);
                adjustmentReason = amount > 0 ? "Adjusted to prevent negative net pay" : "Insufficient wages; deduction skipped";
                result.HasAdjustments = true;
            }

            if (amount > 0 || (amount == 0 && originalAmount > 0))
            {
                result.DeductionLines.Add(new DeductionResultLine
                {
                    Description = deduction.Description,
                    Type = deduction.Type,
                    Amount = amount,
                    AdjustmentReason = adjustmentReason
                });

                postTaxAmount += amount;
            }
        }

        result.TotalPostTaxDeductions = Math.Round(postTaxAmount, 2);
        result.TotalDeductions = Math.Round(result.TotalPreTaxDeductions + result.TotalPostTaxDeductions, 2);

        return result;
    }

    /// <summary>
    /// Calculates the deduction amount based on type and value.
    /// </summary>
    private decimal CalculateDeductionAmount(DeductionInput deduction, decimal grossPay)
    {
        return deduction.Type switch
        {
            DeductionType.PreTaxFixedAmount or DeductionType.PostTaxFixedAmount => deduction.Amount,
            DeductionType.PreTaxPercentage or DeductionType.PostTaxPercentage => Math.Round(grossPay * (deduction.Amount / 100), 2),
            _ => 0
        };
    }
}