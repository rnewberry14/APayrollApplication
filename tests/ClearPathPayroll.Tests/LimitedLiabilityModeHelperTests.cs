using ClearPathPayroll.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace ClearPathPayroll.Tests;

public class LimitedLiabilityModeHelperTests
{
    [Fact]
    public void ShouldUseLocalOnlyMode_ReturnsTrueInDevelopment()
    {
        var environment = new TestHostEnvironment { EnvironmentName = Environments.Development };
        var options = new LimitedLiabilityModeOptions { Enabled = false };

        var result = LimitedLiabilityModeHelper.ShouldUseLocalOnlyMode(environment, options, localPrototypeEnabled: false);

        Assert.True(result);
    }

    [Fact]
    public void ValidateStartupSafety_AllowsSafeLocalDemoDefaults()
    {
        var options = new LimitedLiabilityModeOptions
        {
            Enabled = true,
            AllowExternalTaxApiLookup = false,
            AllowRealAchSubmission = false,
            AllowRealTaxFiling = false,
            AllowTelemetry = false,
            LocalDatabaseProvider = LimitedLiabilityModeHelper.SqlServerLocalDbProvider,
            LocalDatabaseName = "ClearPathPayroll.LocalOnly.Test"
        };

        LimitedLiabilityModeHelper.ValidateStartupSafety(options, localDemoMode: true);
    }

    [Theory]
    [InlineData(nameof(LimitedLiabilityModeOptions.AllowTelemetry))]
    [InlineData(nameof(LimitedLiabilityModeOptions.AllowRealAchSubmission))]
    [InlineData(nameof(LimitedLiabilityModeOptions.AllowRealTaxFiling))]
    public void ValidateStartupSafety_BlocksUnsafeLocalDemoOptions(string unsafeOption)
    {
        var options = new LimitedLiabilityModeOptions
        {
            Enabled = true,
            LocalDatabaseProvider = LimitedLiabilityModeHelper.SqlServerLocalDbProvider,
            LocalDatabaseName = "ClearPathPayroll.LocalOnly.Test"
        };

        if (unsafeOption == nameof(LimitedLiabilityModeOptions.AllowTelemetry))
        {
            options.AllowTelemetry = true;
        }
        else if (unsafeOption == nameof(LimitedLiabilityModeOptions.AllowRealAchSubmission))
        {
            options.AllowRealAchSubmission = true;
        }
        else if (unsafeOption == nameof(LimitedLiabilityModeOptions.AllowRealTaxFiling))
        {
            options.AllowRealTaxFiling = true;
        }

        Assert.Throws<InvalidOperationException>(() =>
            LimitedLiabilityModeHelper.ValidateStartupSafety(options, localDemoMode: true));
    }

    [Fact]
    public void GetLocalDbConnectionString_UsesRequestedDatabaseName()
    {
        var connectionString = LimitedLiabilityModeHelper.GetLocalDbConnectionString("ClearPathPayroll.LocalOnly.Test");

        Assert.Contains("Server=(localdb)\\mssqllocaldb", connectionString);
        Assert.Contains("Database=ClearPathPayroll.LocalOnly.Test", connectionString);
        Assert.Contains("Trusted_Connection=True", connectionString);
    }

    [Fact]
    public void GetSqliteConnectionString_UsesAppDataDatabasePath()
    {
        var environment = new TestHostEnvironment
        {
            EnvironmentName = Environments.Development,
            ContentRootPath = AppContext.BaseDirectory
        };

        var connectionString = LimitedLiabilityModeHelper.GetSqliteConnectionString(environment, "ClearPathPayroll.LocalOnly.Test");

        Assert.Contains("Data Source=", connectionString);
        Assert.Contains("App_Data", connectionString);
        Assert.Contains("ClearPathPayroll.LocalOnly.Test.db", connectionString);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ContentRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
