# QuickBooks Online File Import

QuickBooks Online File Import stages locally exported Excel, CSV, or tab-delimited files from QuickBooks Online. It does not implement OAuth, use the QuickBooks Online API, or store Intuit credentials.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Templates

- Employee list
- Payroll summary
- Payroll details
- Time activities placeholder
- Tax payments placeholder

## Steps

1. Open the report in QuickBooks Online.
2. Export the report to Excel or CSV.
3. Save the exported file locally.
4. Open `/import/quickbooks-online`.
5. Select the matching template.
6. Select the exported local file.
7. Review mappings and aliases.
8. Preview rows.
9. Validate mappings.
10. Confirm the local import batch.

## Cautions

- No OAuth flow is implemented.
- No direct Intuit API is used.
- No Intuit credentials are stored.
- Files are not uploaded to cloud storage.
- Imported data is user-provided and requires review.
