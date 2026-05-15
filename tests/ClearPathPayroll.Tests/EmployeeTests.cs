using ClearPathPayroll.Domain;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for the Employee entity validation rules.
/// </summary>
public class EmployeeTests
{
    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void Employee_ValidHourlyModel_ShouldPassValidation()
    {
        // Arrange
        var employee = new Employee
        {
            CompanyId = 1,
            FirstName = "John",
            LastName = "Doe",
            SSNLast4 = "1234",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            EmploymentStatus = EmploymentStatus.Active,
            PayType = PayType.Hourly,
            HourlyRate = 25.00m,
            ResidenceAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101",
            Email = "john.doe@example.com",
            Phone = "405-123-4567"
        };

        // Act
        var results = ValidateModel(employee);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Employee_ValidSalaryModel_ShouldPassValidation()
    {
        // Arrange
        var employee = new Employee
        {
            CompanyId = 1,
            FirstName = "Jane",
            LastName = "Smith",
            SSNLast4 = "5678",
            DateOfBirth = new DateTime(1985, 5, 15),
            HireDate = new DateTime(2019, 6, 1),
            EmploymentStatus = EmploymentStatus.Active,
            PayType = PayType.Salary,
            AnnualSalary = 60000.00m,
            ResidenceAddress = "456 Oak Ave",
            City = "Tulsa",
            State = "OK",
            ZipCode = "74101",
            Email = "jane.smith@example.com",
            Phone = "918-987-6543"
        };

        // Act
        var results = ValidateModel(employee);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Employee_MissingFirstName_ShouldFailValidation()
    {
        // Arrange
        var employee = new Employee
        {
            CompanyId = 1,
            // FirstName missing
            LastName = "Doe",
            SSNLast4 = "1234",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            PayType = PayType.Hourly,
            HourlyRate = 25.00m,
            ResidenceAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101"
        };

        // Act
        var results = ValidateModel(employee);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("FirstName"));
    }

    [Fact]
    public void Employee_InvalidSSNLast4_ShouldFailValidation()
    {
        // Arrange
        var employee = new Employee
        {
            CompanyId = 1,
            FirstName = "John",
            LastName = "Doe",
            SSNLast4 = "123", // Only 3 digits
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            PayType = PayType.Hourly,
            HourlyRate = 25.00m,
            ResidenceAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101"
        };

        // Act
        var results = ValidateModel(employee);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("SSNLast4"));
    }

    [Fact]
    public void Employee_InvalidEmail_ShouldFailValidation()
    {
        // Arrange
        var employee = new Employee
        {
            CompanyId = 1,
            FirstName = "John",
            LastName = "Doe",
            SSNLast4 = "1234",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            PayType = PayType.Hourly,
            HourlyRate = 25.00m,
            ResidenceAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101",
            Email = "invalid-email"
        };

        // Act
        var results = ValidateModel(employee);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("Email"));
    }

    [Fact]
    public void Employee_NegativeHourlyRate_ShouldFailValidation()
    {
        // Arrange
        var employee = new Employee
        {
            CompanyId = 1,
            FirstName = "John",
            LastName = "Doe",
            SSNLast4 = "1234",
            DateOfBirth = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1),
            PayType = PayType.Hourly,
            HourlyRate = -10.00m, // Negative
            ResidenceAddress = "123 Main St",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73101"
        };

        // Act
        var results = ValidateModel(employee);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage.Contains("HourlyRate"));
    }
}