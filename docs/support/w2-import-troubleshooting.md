# W-2 Import Troubleshooting

Use this guide when W-2 spreadsheet rows do not validate or save.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Common Errors

- **Required W-2 field is not mapped.**
  Map all required fields before validation.

- **EmployeeSSNLast4 requires exactly four digits.**
  Use last four digits only.

- **Full SSN columns are not accepted for W-2 import.**
  Remove or unmap full SSN columns.

- **W-2 wage and tax amounts cannot be negative.**
  Review amount columns in the spreadsheet.

## Troubleshooting Steps

1. Confirm the file has a header row.
2. Confirm the file is CSV, tab-delimited text, or `.xlsx`.
3. Confirm mappings for required fields.
4. Remove full SSN columns.
5. Review amount values.
6. Validate again.

## Cautions

- This workflow does not OCR PDF forms.
- This workflow does not create tax filings.
- This workflow does not validate W-2 correctness.
