using ClearPathPayroll.Domain;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for the Company entity validation rules.
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
}