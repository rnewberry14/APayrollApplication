using ClearPathPayroll.Configuration;
using Xunit;

namespace ClearPathPayroll.Tests;

public class SecureConfigurationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ENTER_PRODUCTION_KEY")]
    [InlineData("replace-with-real-value")]
    [InlineData("placeholder-value")]
    public void ProductionSecretValidation_RejectsInvalidPlaceholders(string? value)
    {
        Assert.False(SecretValidationHelper.IsProductionValueConfigured(value));
    }

    [Theory]
    [InlineData("https://api.example.com")]
    [InlineData("RealSecretKey123")]
    [InlineData("MyVaultUri")]
    public void ProductionSecretValidation_AllowsRealValues(string value)
    {
        Assert.True(SecretValidationHelper.IsProductionValueConfigured(value));
    }

    [Fact]
    public void EncryptionOptions_HasConfiguredKey_ReturnsTrueWhenLocalKeyIsSet()
    {
        var options = new EncryptionOptions
        {
            LocalDevelopmentKey = "my-local-key"
        };

        Assert.True(options.HasConfiguredKey());
    }

    [Fact]
    public void EncryptionOptions_HasConfiguredKey_ReturnsTrueWhenKeyVaultUriIsSet()
    {
        var options = new EncryptionOptions
        {
            KeyVaultUri = "https://my-key-vault.vault.azure.net/"
        };

        Assert.True(options.HasConfiguredKey());
    }
}
