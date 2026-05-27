# PayrollTaxAPI Sandbox Troubleshooting

## What this feature does

The PayrollTaxAPI sandbox integration sends payroll request details to a vendor sandbox API and maps the vendor's tax calculation response into the application.

## Who should use this guide

- Support engineers diagnosing tax calculation failures
- QA testers validating sandbox integration
- Administrators checking sandbox connectivity

## Step-by-step troubleshooting

1. Confirm the application configuration includes `TaxApi:BaseUrl`, `TaxApi:ApiKey`, and `TaxApi:SandboxMode`.
2. Make sure sandbox mode is enabled only in development or staging.
3. Look for `PayrollTaxAPI request:` log entries in the application log.
4. If the request failed, verify the HTTP status and vendor error message in logs.
5. If the response is malformed, validate the sandbox endpoint returns JSON.

## Common errors

- `Tax calculation failed with status ...` — vendor endpoint rejected the request.
- `Unable to contact the tax calculation service.` — connection error or invalid sandbox URL.
- `Received malformed tax calculation response.` — unexpected response format from vendor.

## Recommended fixes

- Verify the sandbox URL is correct and reachable.
- Confirm the API key is valid and not expired.
- Check that the sandbox endpoint accepts `application/json` POST requests at `/calculate`.

## Security cautions

- Do not expose API keys in public logs.
- Do not include SSNs or full addresses in support tickets.
- Treat sandbox credentials as sensitive data.
