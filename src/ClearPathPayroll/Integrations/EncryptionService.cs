using ClearPathPayroll.Configuration;
using Microsoft.Extensions.Options;

namespace ClearPathPayroll.Integrations;

/// <summary>
/// Mock implementation of encryption service. Placeholder - does not actually encrypt.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly EncryptionOptions _options;

    public EncryptionService(IOptions<EncryptionOptions> options)
    {
        _options = options.Value;
    }

    public Task<string> EncryptAsync(string plainText)
    {
        // Placeholder: Return base64 encoded for now (not secure).
        // Encryption options are available for future secure key management.
        var keyHint = string.IsNullOrWhiteSpace(_options.LocalDevelopmentKey) ? _options.KeyVaultUri : _options.LocalDevelopmentKey;
        return Task.FromResult(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText)));
    }

    public Task<string> DecryptAsync(string encryptedText)
    {
        // Placeholder: Decode base64
        return Task.FromResult(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedText)));
    }
}