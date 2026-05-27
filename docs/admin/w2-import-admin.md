# W-2 Import Admin Notes

The W-2 import workflow stages spreadsheet rows, validates required fields, and stores local historical records only after confirmation.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Route

- `/import/w2`

## Required Permissions

- Access to import screens
- Local file read access
- Local database write access through the application

## Storage

Records are saved in `W2HistoricalRecords` and marked as `historical user-entered data`. Employer EIN values are masked. Full employee SSNs are not stored.

## Cautions

- No PDF OCR is performed.
- No tax filing is created.
- No external transmission is performed.
- No correctness verification claim is made.
