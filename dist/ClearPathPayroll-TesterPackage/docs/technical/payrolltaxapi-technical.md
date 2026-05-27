# PayrollTaxAPI Sandbox Integration Technical Notes

## Feature summary

This feature implements a sandbox integration with PayrollTaxAPI.com using `ITaxCalculationService`.

## Architecture

- Uses `HttpClientFactory` to create a typed `PayrollTaxApiCalculationService`.
- Reads `TaxApi` configuration through `TaxApiOptions`.
- Sends a GET request to the `rates/lookup` endpoint with query parameters for work state and pay date.
- Uses `Authorization: Bearer {ApiKey}` for vendor authentication.
- Maps vendor rate data to internal `TaxCalculationResponse` line items.
- Uses `ILogger` for safe request and error logging.

## Request/response mapping

### Request mapping

- `TaxCalculationRequest` fields are converted into a vendor request payload.
- Employee details are mapped without including sensitive street or ZIP details.
- Sandbox mode is passed through configuration and optionally via request metadata.

### Response mapping

- Vendor employee tax lines map to `TaxLineResult`.
- Vendor employer tax lines map to `EmployerTaxLineResult`.
- `ExternalApiReference` and `ApiVersion` are preserved for audit.

## Configuration

- `TaxApi:BaseUrl` - vendor sandbox endpoint
- `TaxApi:ApiKey` - secure API credential
- `TaxApi:SandboxMode` - toggles sandbox integration behavior

## Error handling

- HTTP failures return `IsSuccessful = false` with a safe error message.
- JSON parsing errors are trapped and handled gracefully.
- No sensitive request payloads or API keys are written to logs.

## Testing

- Unit tests cover successful response mapping, HTTP failure handling, and sandbox header behavior.
- Integration placeholder test is included but skipped by default.

## Security cautions

- Never hard-code API keys.
- Validate configuration on startup and use secure stores for secrets.
- Avoid logging SSNs, full addresses, or API secrets.
