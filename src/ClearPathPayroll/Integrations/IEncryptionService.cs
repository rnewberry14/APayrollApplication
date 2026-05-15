namespace ClearPathPayroll.Integrations;

/// <summary>
/// Interface for encryption services. Placeholder for SSN encryption.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the given plain text.
    /// </summary>
    Task<string> EncryptAsync(string plainText);

    /// <summary>
    /// Decrypts the given encrypted text.
    /// </summary>
    Task<string> DecryptAsync(string encryptedText);
}