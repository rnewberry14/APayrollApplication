# Secure Configuration Troubleshooting

## Purpose

This guide helps support staff troubleshoot secure configuration issues for ClearPath Payroll.

## Common failure modes

- Missing production secret values in Azure Key Vault.
- Development user secrets not initialized.
- Placeholder values left in configuration instead of real secrets.

## Troubleshooting steps

1. Confirm the app is reading environment-specific configuration:
   - `ASPNETCORE_ENVIRONMENT=Development` for local development
   - `ASPNETCORE_ENVIRONMENT=Staging` for staging
   - No environment or `Production` for production
2. For local development, verify user secrets:
   - Run `dotnet user-secrets list` in `src/ClearPathPayroll`
3. For production, verify the `AZURE_KEY_VAULT_URI` environment variable.
4. Inspect startup logs for `OptionsValidationException` or missing required settings.
5. If the error mentions ACH production submission, confirm `ACHApi:AllowProductionSubmission` is enabled only in production.

## What to ask the customer

- Are you running the app locally or in production?
- Did you configure User Secrets or Azure Key Vault?
- Did the error mention a missing section such as `TaxApi`, `ACHApi`, or `ConnectionStrings`?

## Support-safe guidance

> Do not ask the customer to share raw secrets, API keys, connection strings, or tokens. Ask for configuration names only and verify the store configuration instead.
