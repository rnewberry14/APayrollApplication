using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for the PayScheduleService calculation methods.
/// </summary>
public class PayScheduleServiceTests
{
    private PayScheduleService CreateService()
    {
        // Since service doesn't use DB for calculations, we can instantiate directly
        return new PayScheduleService(null!); // Null context for testing calculations
    }

    [Fact]
    public void CalculateNextPayPeriod_Weekly_ShouldAdvanceBy7Days()
    {
        // Arrange
        var service = CreateService();
        var paySchedule = new PaySchedule
        {
            Frequency = PayFrequency.Weekly,
            NextPayDate = new DateTime(2023, 1, 1), // Sunday
            NextPeriodStartDate = new DateTime(2022, 12, 25),
            NextPeriodEndDate = new DateTime(2022, 12, 31)
        };

        // Act
        service.CalculateNextPayPeriod(paySchedule);

        // Assert
        Assert.Equal(new DateTime(2023, 1, 8), paySchedule.NextPayDate);
        Assert.Equal(new DateTime(2023, 1, 1), paySchedule.NextPeriodStartDate);
        Assert.Equal(new DateTime(2023, 1, 7), paySchedule.NextPeriodEndDate);
    }

    [Fact]
    public void CalculateNextPayPeriod_Biweekly_ShouldAdvanceBy14Days()
    {
        // Arrange
        var service = CreateService();
        var paySchedule = new PaySchedule
        {
            Frequency = PayFrequency.Biweekly,
            NextPayDate = new DateTime(2023, 1, 1),
            NextPeriodStartDate = new DateTime(2022, 12, 18),
            NextPeriodEndDate = new DateTime(2022, 12, 31)
        };

        // Act
        service.CalculateNextPayPeriod(paySchedule);

        // Assert
        Assert.Equal(new DateTime(2023, 1, 15), paySchedule.NextPayDate);
        Assert.Equal(new DateTime(2023, 1, 1), paySchedule.NextPeriodStartDate);
        Assert.Equal(new DateTime(2023, 1, 14), paySchedule.NextPeriodEndDate);
    }

    [Fact]
    public void CalculateNextPayPeriod_Semimonthly_From15thToEndOfMonth()
    {
        // Arrange
        var service = CreateService();
        var paySchedule = new PaySchedule
        {
            Frequency = PayFrequency.Semimonthly,
            NextPayDate = new DateTime(2023, 1, 15),
            NextPeriodStartDate = new DateTime(2023, 1, 1),
            NextPeriodEndDate = new DateTime(2023, 1, 15)
        };

        // Act
        service.CalculateNextPayPeriod(paySchedule);

        // Assert
        Assert.Equal(new DateTime(2023, 1, 31), paySchedule.NextPayDate); // End of January
        Assert.Equal(new DateTime(2023, 1, 16), paySchedule.NextPeriodStartDate);
        Assert.Equal(new DateTime(2023, 1, 31), paySchedule.NextPeriodEndDate);
    }

    [Fact]
    public void CalculateNextPayPeriod_Semimonthly_FromEndOfMonthTo15th()
    {
        // Arrange
        var service = CreateService();
        var paySchedule = new PaySchedule
        {
            Frequency = PayFrequency.Semimonthly,
            NextPayDate = new DateTime(2023, 1, 31),
            NextPeriodStartDate = new DateTime(2023, 1, 16),
            NextPeriodEndDate = new DateTime(2023, 1, 31)
        };

        // Act
        service.CalculateNextPayPeriod(paySchedule);

        // Assert
        Assert.Equal(new DateTime(2023, 2, 15), paySchedule.NextPayDate); // 15th of February
        Assert.Equal(new DateTime(2023, 2, 1), paySchedule.NextPeriodStartDate);
        Assert.Equal(new DateTime(2023, 2, 15), paySchedule.NextPeriodEndDate);
    }

    [Fact]
    public void CalculateNextPayPeriod_Monthly_ShouldAdvanceBy1Month()
    {
        // Arrange
        var service = CreateService();
        var paySchedule = new PaySchedule
        {
            Frequency = PayFrequency.Monthly,
            NextPayDate = new DateTime(2023, 1, 15),
            NextPeriodStartDate = new DateTime(2022, 12, 16),
            NextPeriodEndDate = new DateTime(2023, 1, 15)
        };

        // Act
        service.CalculateNextPayPeriod(paySchedule);

        // Assert
        Assert.Equal(new DateTime(2023, 2, 15), paySchedule.NextPayDate);
        Assert.Equal(new DateTime(2023, 1, 16), paySchedule.NextPeriodStartDate);
        Assert.Equal(new DateTime(2023, 2, 15), paySchedule.NextPeriodEndDate);
    }

    [Fact]
    public void CalculateNextPayPeriod_Monthly_EndOfMonthEdgeCase()
    {
        // Arrange - Pay on last day of January (31st)
        var service = CreateService();
        var paySchedule = new PaySchedule
        {
            Frequency = PayFrequency.Monthly,
            NextPayDate = new DateTime(2023, 1, 31),
            NextPeriodStartDate = new DateTime(2022, 12, 16),
            NextPeriodEndDate = new DateTime(2023, 1, 31)
        };

        // Act
        service.CalculateNextPayPeriod(paySchedule);

        // Assert - Should go to last day of February (28th in 2023)
        Assert.Equal(new DateTime(2023, 2, 28), paySchedule.NextPayDate);
        Assert.Equal(new DateTime(2023, 2, 1), paySchedule.NextPeriodStartDate);
        Assert.Equal(new DateTime(2023, 2, 28), paySchedule.NextPeriodEndDate);
    }

    [Fact]
    public void CalculateNextPayPeriod_Monthly_EndOfMonthLeapYear()
    {
        // Arrange - Pay on last day of February in leap year (29th)
        var service = CreateService();
        var paySchedule = new PaySchedule
        {
            Frequency = PayFrequency.Monthly,
            NextPayDate = new DateTime(2024, 2, 29),
            NextPeriodStartDate = new DateTime(2024, 1, 16),
            NextPeriodEndDate = new DateTime(2024, 2, 29)
        };

        // Act
        service.CalculateNextPayPeriod(paySchedule);

        // Assert - Should go to last day of March (31st)
        Assert.Equal(new DateTime(2024, 3, 31), paySchedule.NextPayDate);
        Assert.Equal(new DateTime(2024, 3, 1), paySchedule.NextPeriodStartDate);
        Assert.Equal(new DateTime(2024, 3, 31), paySchedule.NextPeriodEndDate);
    }
}