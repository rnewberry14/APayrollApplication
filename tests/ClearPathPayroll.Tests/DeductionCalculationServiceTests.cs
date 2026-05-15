using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for DeductionCalculationService.
/// </summary>
public class DeductionCalculationServiceTests
{
    private DeductionCalculationService CreateService()
    {
        return new DeductionCalculationService();
    }

    [Fact]
    public void CalculateDeductions_NoDeductions_ReturnsZero()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>();

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(0, result.TotalPreTaxDeductions);
        Assert.Equal(0, result.TotalPostTaxDeductions);
        Assert.Equal(0, result.TotalDeductions);
        Assert.False(result.HasAdjustments);
    }

    [Fact]
    public void CalculateDeductions_PreTaxFixedAmount_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "401k", Type = DeductionType.PreTaxFixedAmount, Amount = 100m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(100m, result.TotalPreTaxDeductions);
        Assert.Equal(0, result.TotalPostTaxDeductions);
        Assert.Equal(100m, result.TotalDeductions);
        Assert.False(result.HasAdjustments);
        Assert.Single(result.DeductionLines);
        Assert.Equal("401k", result.DeductionLines[0].Description);
        Assert.Equal(100m, result.DeductionLines[0].Amount);
    }

    [Fact]
    public void CalculateDeductions_PreTaxPercentage_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "401k 5%", Type = DeductionType.PreTaxPercentage, Amount = 5m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(50m, result.TotalPreTaxDeductions);
        Assert.Equal(0, result.TotalPostTaxDeductions);
        Assert.Equal(50m, result.TotalDeductions);
        Assert.False(result.HasAdjustments);
    }

    [Fact]
    public void CalculateDeductions_PostTaxFixedAmount_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "Health Insurance", Type = DeductionType.PostTaxFixedAmount, Amount = 50m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(0, result.TotalPreTaxDeductions);
        Assert.Equal(50m, result.TotalPostTaxDeductions);
        Assert.Equal(50m, result.TotalDeductions);
        Assert.False(result.HasAdjustments);
    }

    [Fact]
    public void CalculateDeductions_PostTaxPercentage_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "Charity 2%", Type = DeductionType.PostTaxPercentage, Amount = 2m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(0, result.TotalPreTaxDeductions);
        Assert.Equal(20m, result.TotalPostTaxDeductions); // 2% of 1000
        Assert.Equal(20m, result.TotalDeductions);
        Assert.False(result.HasAdjustments);
    }

    [Fact]
    public void CalculateDeductions_Mixed_AllTypes_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "401k Fixed", Type = DeductionType.PreTaxFixedAmount, Amount = 100m },
            new() { Description = "HSA 3%", Type = DeductionType.PreTaxPercentage, Amount = 3m },
            new() { Description = "Health Insurance", Type = DeductionType.PostTaxFixedAmount, Amount = 50m },
            new() { Description = "Charity 1%", Type = DeductionType.PostTaxPercentage, Amount = 1m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(130m, result.TotalPreTaxDeductions); // 100 + (1000 * 3%)
        Assert.Equal(60m, result.TotalPostTaxDeductions); // 50 + (1000 * 1%)
        Assert.Equal(190m, result.TotalDeductions);
        Assert.False(result.HasAdjustments);
        Assert.Equal(4, result.DeductionLines.Count);
    }

    [Fact]
    public void CalculateDeductions_PreTaxExceedsGrossPay_AdjustsDeduction()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 500m;
        var netPay = 400m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "401k", Type = DeductionType.PreTaxFixedAmount, Amount = 600m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(500m, result.TotalPreTaxDeductions); // Capped at gross pay
        Assert.True(result.HasAdjustments);
        Assert.Contains("not exceed gross pay", result.DeductionLines[0].AdjustmentReason);
    }

    [Fact]
    public void CalculateDeductions_PostTaxExceedsNetPay_AdjustsDeduction()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 100m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "Health Insurance", Type = DeductionType.PostTaxFixedAmount, Amount = 200m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(100m, result.TotalPostTaxDeductions); // Capped at net pay
        Assert.True(result.HasAdjustments);
        Assert.Contains("negative net pay", result.DeductionLines[0].AdjustmentReason);
    }

    [Fact]
    public void CalculateDeductions_InsufficientWages_SkipsDeduction()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 50m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "Health Insurance", Type = DeductionType.PostTaxFixedAmount, Amount = 100m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(50m, result.TotalPostTaxDeductions); // All available net pay
        Assert.True(result.HasAdjustments);
    }

    [Fact]
    public void CalculateDeductions_MultiplePostTaxDeductions_SkipsIfInsufficientFunds()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 120m; // Only 120 available for post-tax
        var deductions = new List<DeductionInput>
        {
            new() { Description = "Health", Type = DeductionType.PostTaxFixedAmount, Amount = 100m },
            new() { Description = "Dental", Type = DeductionType.PostTaxFixedAmount, Amount = 50m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(120m, result.TotalPostTaxDeductions);
        Assert.True(result.HasAdjustments);
        // First deduction gets 100, second gets 20 (capped)
        Assert.Equal(100m, result.DeductionLines[0].Amount);
        Assert.Equal(20m, result.DeductionLines[1].Amount);
    }

    [Fact]
    public void CalculateDeductions_PreTaxAndPostTaxTogether_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 700m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "401k", Type = DeductionType.PreTaxFixedAmount, Amount = 100m },
            new() { Description = "Health", Type = DeductionType.PostTaxFixedAmount, Amount = 50m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(100m, result.TotalPreTaxDeductions);
        Assert.Equal(50m, result.TotalPostTaxDeductions);
        Assert.Equal(150m, result.TotalDeductions);
        Assert.False(result.HasAdjustments);
    }

    [Fact]
    public void CalculateDeductions_ZeroDeductionAmount_HasNoEffect()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "Optional", Type = DeductionType.PreTaxFixedAmount, Amount = 0m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(0, result.TotalPreTaxDeductions);
        Assert.Equal(0, result.TotalDeductions);
    }

    [Fact]
    public void CalculateDeductions_RoundsToTwoDecimals()
    {
        // Arrange
        var service = CreateService();
        var grossPay = 1000m;
        var netPay = 800m;
        var deductions = new List<DeductionInput>
        {
            new() { Description = "3.333%", Type = DeductionType.PreTaxPercentage, Amount = 3.333m }
        };

        // Act
        var result = service.CalculateDeductions(grossPay, netPay, deductions);

        // Assert
        Assert.Equal(33.33m, result.TotalPreTaxDeductions);
    }
}