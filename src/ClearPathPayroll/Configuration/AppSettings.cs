using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Configuration;

public class ConnectionStringsOptions
{
    [Required(ErrorMessage = "DefaultConnection is required.")]
    public string DefaultConnection { get; set; } = string.Empty;
}

public class TaxApiOptions
{
    [Required(ErrorMessage = "Tax API base URL is required.")]
    public string BaseUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tax API key is required.")]
    public string ApiKey { get; set; } = string.Empty;

    public bool SandboxMode { get; set; } = true;
}

public class AchApiOptions
{
    [Required(ErrorMessage = "ACH API base URL is required.")]
    public string BaseUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "ACH API key is required.")]
    public string ApiKey { get; set; } = string.Empty;

    public bool SandboxMode { get; set; } = true;

    public bool AllowProductionSubmission { get; set; }
}

public class BankVerificationOptions
{
    public string Provider { get; set; } = "Default";

    [Required(ErrorMessage = "Bank verification base URL is required.")]
    public string BaseUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bank verification API key is required.")]
    public string ApiKey { get; set; } = string.Empty;
}

public class EncryptionOptions
{
    public string LocalDevelopmentKey { get; set; } = string.Empty;

    public string KeyVaultUri { get; set; } = string.Empty;

    public string KeyVaultKeyName { get; set; } = string.Empty;

    public bool HasConfiguredKey() =>
        !string.IsNullOrWhiteSpace(LocalDevelopmentKey) ||
        !string.IsNullOrWhiteSpace(KeyVaultUri);
}

public class AuthenticationOptions
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    [Required(ErrorMessage = "Authentication authority is required.")]
    public string Authority { get; set; } = string.Empty;
}

public class PrototypeModeOptions
{
    public bool Enabled { get; set; }

    [Required(ErrorMessage = "LocalDbDatabaseName is required for Prototype Mode.")]
    public string LocalDbDatabaseName { get; set; } = "ClearPathPayroll.LocalPrototype";
}

public class LimitedLiabilityModeOptions
{
    public bool Enabled { get; set; }

    public bool AllowExternalTaxApiLookup { get; set; }

    public bool AllowRealAchSubmission { get; set; }

    public bool AllowRealTaxFiling { get; set; }

    public bool AllowTelemetry { get; set; }

    [Required(ErrorMessage = "LocalDatabaseProvider is required for Limited Liability Mode.")]
    public string LocalDatabaseProvider { get; set; } = LimitedLiabilityModeHelper.SqlServerLocalDbProvider;

    [Required(ErrorMessage = "LocalDatabaseName is required for Limited Liability Mode.")]
    public string LocalDatabaseName { get; set; } = "ClearPathPayroll.LocalOnly";
}

public class EmailOptions
{
    public string Provider { get; set; } = "smtp";

    public string FromAddress { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}

public class AzureKeyVaultOptions
{
    public bool Enabled { get; set; }

    public string VaultUri { get; set; } = string.Empty;
}

public class AzureStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
