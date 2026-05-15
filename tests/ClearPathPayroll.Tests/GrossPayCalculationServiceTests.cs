using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for GrossPayCalculationService.
/// </summary>
public class GrossPayCalculationServiceTests
{
    private GrossPayCalculationService CreateService()
    {
        return new GrossPayCalculationService();
    }

    [Fact]
    public void CalculateGrossPay_SalaryWeekly_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Salary,
            AnnualSalary = 52000.00m
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Weekly };

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule);

        // Assert
        Assert.Equal(PayType.Salary, result.PayType);
        Assert.Equal(PayFrequency.Weekly, result.PayFrequency);
        Assert.Equal(1000.00m, result.TotalGrossPay); // 52000 / 52
        Assert.Equal(1000.00m, result.RegularPay);
        Assert.Equal(0, result.RegularHours);
        Assert.Equal(0, result.OvertimeHours);
        Assert.Equal(0, result.OvertimePay);
    }

    [Fact]
    public void CalculateGrossPay_SalaryBiweekly_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Salary,
            AnnualSalary = 52000.00m
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Biweekly };

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule);

        // Assert
        Assert.Equal(2000.00m, result.TotalGrossPay); // 52000 / 26
    }

    [Fact]
    public void CalculateGrossPay_SalarySemimonthly_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Salary,
            AnnualSalary = 52000.00m
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Semimonthly };

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule);

        // Assert
        Assert.Equal(2166.67m, result.TotalGrossPay); // 52000 / 24
    }

    [Fact]
    public void CalculateGrossPay_SalaryMonthly_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Salary,
            AnnualSalary = 52000.00m
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Monthly };

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule);

        // Assert
        Assert.Equal(4333.33m, result.TotalGrossPay); // 52000 / 12
    }

    [Fact]
    public void CalculateGrossPay_HourlyRegular_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Hourly,
            HourlyRate = 25.00m
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Weekly };
        var hoursWorked = 40m;

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule, hoursWorked);

        // Assert
        Assert.Equal(PayType.Hourly, result.PayType);
        Assert.Equal(PayFrequency.Weekly, result.PayFrequency);
        Assert.Equal(40m, result.RegularHours);
        Assert.Equal(0m, result.OvertimeHours);
        Assert.Equal(1000.00m, result.RegularPay);
        Assert.Equal(0m, result.OvertimePay);
        Assert.Equal(1000.00m, result.TotalGrossPay);
    }

    [Fact]
    public void CalculateGrossPay_HourlyWithOvertime_CalculatesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Hourly,
            HourlyRate = 25.00m
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Weekly };
        var hoursWorked = 45m;

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule, hoursWorked);

        // Assert
        Assert.Equal(40m, result.RegularHours);
        Assert.Equal(5m, result.OvertimeHours);
        Assert.Equal(1000.00m, result.RegularPay); // 40 * 25
        Assert.Equal(187.50m, result.OvertimePay); // 5 * 25 * 1.5
        Assert.Equal(1187.50m, result.TotalGrossPay);
    }

    [Fact]
    public void CalculateGrossPay_HourlyNoHoursProvided_UsesZero()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Hourly,
            HourlyRate = 25.00m
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Weekly };

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule); // No hours

        // Assert
        Assert.Equal(0m, result.RegularHours);
        Assert.Equal(0m, result.OvertimeHours);
        Assert.Equal(0m, result.RegularPay);
        Assert.Equal(0m, result.OvertimePay);
        Assert.Equal(0m, result.TotalGrossPay);
    }

    [Fact]
    public void CalculateGrossPay_SalaryNoAnnualSalary_UsesZero()
    {
        // Arrange
        var service = CreateService();
        var employee = new Employee
        {
            PayType = PayType.Salary
            // No AnnualSalary
        };
        var paySchedule = new PaySchedule { Frequency = PayFrequency.Weekly };

        // Act
        var result = service.CalculateGrossPay(employee, paySchedule);

        // Assert
        Assert.Equal(0m, result.TotalGrossPay);
    }
}