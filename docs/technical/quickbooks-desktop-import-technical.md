# QuickBooks Desktop Import Technical Notes

QuickBooks Desktop import uses local file parsing and template-based alias mapping.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Components

- `QuickBooksImportTemplateCatalog`
- `QuickBooksImport.razor`
- `ImportService`
- Existing CSV, tab-delimited, and `.xlsx` parsers

## Templates

Desktop templates include employee list, payroll summary, payroll item detail, paycheck detail, and tax liability/payment report placeholder.

## External Systems

No QuickBooks SDK, QuickBooks Desktop connection, Intuit credentials, cloud upload, telemetry, ACH submission, or tax filing is added.
