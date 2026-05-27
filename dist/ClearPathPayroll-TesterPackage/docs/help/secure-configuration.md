# Secure Configuration Setup

## What this feature does

ClearPath Payroll now separates sensitive secrets from source code and local config files. Local development uses .NET User Secrets for API keys and secret values, while production can use Azure Key Vault.

## Who can use it

- Developers setting up the app locally.
- Administrators deploying the app to staging or production.

## Before you begin

- Confirm .NET 8.0 SDK is installed.
- Confirm the app is running from the `src/ClearPathPayroll` folder.
- Do not store real secrets in Git.

## Steps

1. Use `appsettings.json` only for safe, non-sensitive defaults like logging, base URLs, and non-secret provider names.
2. For local development, add secrets with .NET User Secrets:
   - `dotnet user-secrets init`
   - `dotnet user-secrets set "TaxApi:ApiKey" "YOUR_DEV_TAX_API_KEY"`
   - `dotnet user-secrets set "ACHApi:ApiKey" "YOUR_DEV_ACH_API_KEY"`
   - `dotnet user-secrets set "BankVerification:ApiKey" "YOUR_DEV_BANK_VERIFICATION_API_KEY"`
   - `dotnet user-secrets set "Email:ApiKey" "YOUR_DEV_EMAIL_API_KEY"`
   - `dotnet user-secrets set "Encryption:LocalDevelopmentKey" "YOUR_DEV_ENCRYPTION_KEY"`
3. Confirm `ACHApi:SandboxMode` is enabled in development and `ACHApi:AllowProductionSubmission` is false.
4. For production, configure Azure Key Vault and set `AZURE_KEY_VAULT_URI`.
5. Start the app with `dotnet run`.

## Common errors

- `Required configuration section 'TaxApi' is missing.`
  - Add the section to `appsettings.json` or configure it in user secrets.
- `Required production secret missing.`
  - Verify production secrets are set in Key Vault or environment variables.

## Security caution

> Caution: Do not commit API keys, connection strings, or encryption secrets into Git. Use User Secrets for local development and Azure Key Vault for staging/production.
