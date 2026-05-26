using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for the Company entity and CompanyService.
/// </summary>
public class CompanyTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Company_ValidModel_ShouldPassValidation()
    {
        // Arrange
        var company = new Company
        {
            LegalName = "Test Company Inc.",
            FEIN = "1234567890",
            PrimaryAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101",
            Phone = "405-123-4567",
            Email = "contact@testcompany.com",
            PayrollContactName = "John Doe",
            IsActive = true
        };

        // Act
        var results = ValidateModel(company);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Company_MissingLegalName_ShouldFailValidation()
    {
        // Arrange
        var company = new Company
        {
            // LegalName is missing
            FEIN = "1234567890",
            PrimaryAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101"
        };

        // Act
        var results = ValidateModel(company);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("LegalName"));
    }

    [Fact]
    public void Company_InvalidEmail_ShouldFailValidation()
    {
        // Arrange
        var company = new Company
        {
            LegalName = "Test Company Inc.",
            FEIN = "1234567890",
            PrimaryAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101",
            Email = "invalid-email"
        };

        // Act
        var results = ValidateModel(company);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("Email"));
    }

    [Fact]
    public void Company_FEINTooLong_ShouldFailValidation()
    {
        // Arrange
        var company = new Company
        {
            LegalName = "Test Company Inc.",
            FEIN = "12345678901", // 11 characters, max 10
            PrimaryAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101"
        };

        // Act
        var results = ValidateModel(company);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("FEIN"));
    }

    [Fact]
    public void Company_StateTooLong_ShouldFailValidation()
    {
        // Arrange
        var company = new Company
        {
            LegalName = "Test Company Inc.",
            FEIN = "1234567890",
            PrimaryAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OKA", // 3 characters, max 2
            ZipCode = "73101"
        };

        // Act
        var results = ValidateModel(company);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("State"));
    }

    [Theory]
    [InlineData("1234567890", "12-34-****")]
    [InlineData("12-34-5678", "12-34-****")]
    [InlineData("123456789", "12-34-****")]
    [InlineData("", "XX-XX-****")]
    [InlineData(null, "XX-XX-****")]
    [InlineData("12", "XX-XX-****")]
    public void CompanyService_MaskFEIN_ShouldReturnCorrectMaskedValue(string fein, string expected)
    {
        // Act
        var result = CompanyService.MaskFEIN(fein);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CompanyService_MaskFEIN_WithValidFEIN_ShouldHideMostDigits()
    {
        // Arrange
        var fein = "98-7654321";

        // Act
        var masked = CompanyService.MaskFEIN(fein);

        // Assert
        Assert.StartsWith("98-76", masked);
        Assert.EndsWith("****", masked);
        Assert.DoesNotContain("5", masked); // Middle digits should be masked
    }
}
