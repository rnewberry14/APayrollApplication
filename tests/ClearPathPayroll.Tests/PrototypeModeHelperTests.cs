using ClearPathPayroll.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace ClearPathPayroll.Tests;

public class PrototypeModeHelperTests
{
    [Fact]
    public void ShouldUseLocalPrototypeMode_DevelopmentEnvironment_ReturnsTrue()
    {
        var environment = new TestHostEnvironment { EnvironmentName = Environments.Development };
        var options = new PrototypeModeOptions { Enabled = false };

        var result = PrototypeModeHelper.ShouldUseLocalPrototypeMode(environment, options);

        Assert.True(result);
    }

    [Fact]
    public void ShouldUseLocalPrototypeMode_StagingEnabled_ReturnsTrue()
    {
        var environment = new TestHostEnvironment { EnvironmentName = Environments.Staging };
        var options = new PrototypeModeOptions { Enabled = true };

        var result = PrototypeModeHelper.ShouldUseLocalPrototypeMode(environment, options);

        Assert.True(result);
    }

    [Fact]
    public void ShouldUseLocalPrototypeMode_ProductionEnabled_ReturnsFalse()
    {
        var environment = new TestHostEnvironment { EnvironmentName = Environments.Production };
        var options = new PrototypeModeOptions { Enabled = true };

        var result = PrototypeModeHelper.ShouldUseLocalPrototypeMode(environment, options);

        Assert.False(result);
    }

    [Fact]
    public void GetLocalDbConnectionString_ReturnsLocalDbConnectionString()
    {
        var connectionString = PrototypeModeHelper.GetLocalDbConnectionString("ClearPathPayroll.LocalPrototype.Test");

        Assert.Contains("Server=(localdb)\\mssqllocaldb", connectionString);
        Assert.Contains("Database=ClearPathPayroll.LocalPrototype.Test", connectionString);
        Assert.Contains("Trusted_Connection=True", connectionString);
    }

    [Fact]
    public void GetLocalDbConnectionString_EmptyDatabaseName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => PrototypeModeHelper.GetLocalDbConnectionString(""));
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ContentRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string? WebRootPath { get; set; }
        public Microsoft.Extensions.FileProviders.IFileProvider? WebRootFileProvider { get; set; }
    }
}
