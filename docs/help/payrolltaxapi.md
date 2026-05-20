# PayrollTaxAPI Sandbox Integration

## What this feature does

This feature adds a sandbox integration with PayrollTaxAPI.com for tax calculations. It sends payroll and employee tax request data to the vendor's sandbox endpoint, receives vendor tax line items, and maps the response into the application's `TaxCalculationResponse` model.

## Who can use it

- Payroll administrators configuring tax calculation integration
- Developers testing payroll tax calculations in sandbox mode
- Support staff validating sandbox API connectivity

## How to use it

1. Configure `TaxApi` settings in `appsettings.Development.json`, GitHub secrets, or Azure Key Vault.
2. Set `TaxApi:BaseUrl` to the sandbox endpoint, for example `https://payrolltaxapi.com/v1/`.
3. Set `TaxApi:ApiKey` to the sandbox API key.
4. Leave `TaxApi:SandboxMode` set to `true` for development and staging.
5. The service calls the vendor lookup endpoint using `GET rates/lookup?workState={state}&payDate={date}`.
6. Run payroll calculations normally from the application.

## Required permissions

- Access to the PayrollTaxAPI.com sandbox account
- Ability to store the sandbox API key securely in user secrets or Key Vault
- App Service or development host network access to the sandbox endpoint

## Common errors

- `Tax calculation failed with status ...` — vendor rejected the request or sandbox endpoint returned an error.
- `Unable to contact the tax calculation service.` — network issue, DNS problem, or incorrect sandbox URL.
- `Received malformed tax calculation response.` — vendor returned unexpected JSON.

## Troubleshooting

- Verify `TaxApi:BaseUrl` and `TaxApi:ApiKey` values.
- Ensure sandbox mode is enabled only for non-production environments.
- Confirm the sandbox endpoint is reachable from the application host.
- Check application logs for `PayrollTaxAPI request:` entries without sensitive details.

## Security cautions

- Do not store tax API keys in source code.
- Use user secrets for local development and Key Vault for production secrets.
- Never log SSNs, full employee addresses, or API keys.
- Treat the sandbox API key as sensitive configuration material.
