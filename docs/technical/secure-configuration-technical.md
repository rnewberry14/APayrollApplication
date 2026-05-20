# Secure Configuration Technical Notes

## What this feature does

The app now provides a secure secrets and configuration structure with:
- strong typed options classes for API and secret settings
- environment-specific configuration support
- local .NET User Secrets support for development
- Azure Key Vault integration for production
- startup validation for required production secrets

## Implementation details

- New option classes were added under `src/ClearPathPayroll/Configuration/`.
- `Program.cs` now binds configuration sections and validates them on startup.
- Production uses `AZURE_KEY_VAULT_URI` when available and only loads Key Vault in non-development environments.
- `appsettings.json` contains safe defaults and base URL sections.
- `appsettings.Development.json` and `appsettings.Staging.json` contain safe placeholders only.

## Configuration sections

- `ConnectionStrings`
- `TaxApi`
- `ACHApi`
- `BankVerification`
- `Encryption`
- `Authentication`
- `Email`
- `AzureKeyVault`
- `AzureStorage`

## Startup behavior

- In development, user secrets are loaded if available.
- In production/staging, Azure Key Vault is loaded when `AZURE_KEY_VAULT_URI` is configured.
- Startup fails if required production secrets are missing or placeholder values are still present.
