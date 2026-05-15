using ClearPathPayroll.Integrations;

namespace ClearPathPayroll.Integrations;

/// <summary>
/// Mock implementation of encryption service. Placeholder - does not actually encrypt.
/// </summary>
public class EncryptionService : IEncryptionService
{
    public Task<string> EncryptAsync(string plainText)
    {
        // Placeholder: Return base64 encoded for now (not secure)
        return Task.FromResult(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText)));
    }

    public Task<string> DecryptAsync(string encryptedText)
    {
        // Placeholder: Decode base64
        return Task.FromResult(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedText)));
    }
}