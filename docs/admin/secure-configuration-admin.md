# Secure Configuration Admin Guide

## What this feature does

This secure configuration feature ensures sensitive values are managed outside source control. It supports local development secrets, staging placeholders, and production Azure Key Vault access.

## Who can use it

- System administrators
- DevOps engineers
- Security administrators

## Required permissions

- Access to the Azure subscription and Key Vault for production deployments.
- Permission to manage User Secrets on the local developer machine.

## Step-by-step setup

1. Confirm the app uses `appsettings.json` for non-sensitive defaults only.
2. In development, initialize User Secrets:
   - `dotnet user-secrets init`
3. Set approved local secret values:
   - `dotnet user-secrets set "TaxApi:ApiKey" "YOUR_DEV_TAX_API_KEY"`
   - `dotnet user-secrets set "ACHApi:ApiKey" "YOUR_DEV_ACH_API_KEY"`
   - `dotnet user-secrets set "BankVerification:ApiKey" "YOUR_DEV_BANK_VERIFICATION_API_KEY"`
   - `dotnet user-secrets set "Email:ApiKey" "YOUR_DEV_EMAIL_API_KEY"`
   - `dotnet user-secrets set "Encryption:LocalDevelopmentKey" "YOUR_DEV_ENCRYPTION_KEY"`
4. Ensure `ACHApi:SandboxMode` is enabled for dev and `ACHApi:AllowProductionSubmission` remains false.
5. For production, configure an Azure Key Vault and set `AZURE_KEY_VAULT_URI` in the app service environment.

## What to check if something goes wrong

- Verify `DefaultConnection` is set and not a placeholder in production.
- Verify `TaxApi:ApiKey` and `ACHApi:ApiKey` are configured in Key Vault or `ASPNETCORE_ENVIRONMENT=Development` user secrets.
- Confirm `AZURE_KEY_VAULT_URI` points to a valid Key Vault.

## Troubleshooting

- If the app fails to start in production, check startup logs for missing required secrets.
- If local development fails, confirm your user secrets were applied and that `dotnet user-secrets` is initialized in the project folder.

## Security caution

> Caution: Never place production API keys, database connection strings, or encryption keys in `appsettings.json`. Use secure stores instead.
