# Tax Deposits Import Troubleshooting

Use this guide when tax deposit imports do not validate or save.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Common Errors

- **Required tax deposit field is not mapped.**
  Map all required fields before validation.

- **CompanyIdentifier did not match a local company.**
  Use a company ID, legal name, or FEIN already stored locally.

- **TaxPeriodEnd cannot be before TaxPeriodStart.**
  Review period dates in the source file.

- **Amount must be greater than zero.**
  Enter a positive deposit amount.

## Troubleshooting Steps

1. Confirm the file has a header row.
2. Confirm column mappings.
3. Confirm company identifiers match local company records.
4. Review date formats.
5. Review amount values.
6. Validate again.

## Cautions

- Imported records are user-entered deposit records.
- The workflow does not submit payments.
- The workflow does not file taxes.
- The workflow does not verify deposits with an agency.
