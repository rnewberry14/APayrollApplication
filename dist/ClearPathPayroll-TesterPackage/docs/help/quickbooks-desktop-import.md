# QuickBooks Desktop File Import

QuickBooks Desktop File Import stages reports exported from QuickBooks Desktop as local CSV, Excel, or tab-delimited files. It does not connect directly to QuickBooks Desktop and does not require QuickBooks or Intuit credentials.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Templates

- Employee list
- Payroll summary
- Payroll item detail
- Paycheck detail
- Tax liability/payment report placeholder

## Steps

1. Open the report in QuickBooks Desktop.
2. Export the report to Excel or CSV.
3. Save the exported file locally.
4. Open `/import/quickbooks-desktop`.
5. Select the matching template.
6. Select the exported local file.
7. Review mappings and aliases.
8. Preview rows.
9. Validate mappings.
10. Confirm the local import batch.

## Cautions

- No QuickBooks SDK connection is used.
- No Intuit credentials are stored.
- Files are not transmitted externally.
- The workflow does not submit ACH or tax filings.
- Imported data is user-provided and requires review.
