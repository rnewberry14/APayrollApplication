# PayrollTaxAPI Sandbox Integration Admin Guide

## Feature overview

This admin guide covers the PayrollTaxAPI sandbox integration. The service uses `ITaxCalculationService` and sends payroll data to a vendor sandbox endpoint for tax line item calculation.

## Who should configure this

- Administrators responsible for payroll integration setup
- DevOps engineers deploying configuration to staging or development environments

## Setup steps

1. Open the application configuration file for the current environment.
2. Add or update the `TaxApi` section:
   - `BaseUrl` to the PayrollTaxAPI sandbox endpoint
   - `ApiKey` to the sandbox API key
   - `SandboxMode` to `true`
3. For production use, do not enable sandbox mode and do not store plain text keys in version control.
4. In Azure, place the `TaxApi:ApiKey` secret in Key Vault and ensure the app has access.

## Required permissions

- `Azure Key Vault Secrets User` or equivalent for storing API keys
- App Service managed identity access to Key Vault, if used
- Permission to update application settings in the deployment environment

## Common administrative issues

- Missing `ApiKey` in configuration will prevent `TaxApiOptions` from validating.
- Sandbox mode must remain `true` in non-production environments.
- The base URL must be a valid HTTPS endpoint.

## Troubleshooting

- Review the app service configuration and confirm the `TaxApi` section is present.
- Check for validation errors at application startup.
- Confirm the sandbox base URL is reachable from the deployed environment.
- Use the application logs to verify request summaries are being emitted.

## Security notes

- Never commit `TaxApi:ApiKey` to Git.
- Use secure configuration stores such as Azure Key Vault.
- Avoid logging any sensitive employee data or API credentials.
