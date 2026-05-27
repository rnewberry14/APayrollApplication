# QuickBooks Online Import Technical Notes

QuickBooks Online import uses local file parsing and template-based alias mapping.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Components

- `QuickBooksImportTemplateCatalog`
- `QuickBooksImport.razor`
- `ImportService`
- Existing CSV, tab-delimited, and `.xlsx` parsers

## Templates

Online templates include employee list, payroll summary, payroll details, time activities placeholder, and tax payments placeholder.

## External Systems

No OAuth, direct Intuit API, Intuit credential storage, cloud upload, telemetry, ACH submission, or tax filing is added.
