# Checks Import Troubleshooting

Use this guide when imported checks do not validate or save.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Common Errors

- **Required check field is not mapped.**
  Map all required check fields before validation.

- **CompanyIdentifier did not match a local company.**
  Use a local company ID, legal name, or FEIN value already stored in the local database.

- **EmployeeIdentifier did not match a local employee.**
  For `/import/checks`, the employee record needs to exist before import.

- **Gross pay minus employee taxes and deductions does not match NetPay within rounding tolerance.**
  Review gross pay, taxes, deductions, and net pay values in the file.

- **DirectDepositLast4 requires exactly four digits.**
  Use only the last four digits for direct deposit references.

## Troubleshooting Steps

1. Confirm the file has a header row.
2. Confirm mappings match the expected check fields.
3. Confirm company identifiers match local company records.
4. For checks-only imports, confirm employee identifiers match local employee records.
5. Review rows flagged for net pay mismatch.
6. Remove full SSNs, routing numbers, and full bank account numbers.
7. Validate again.

## Cautions

- Imported check rows are user-provided records.
- The workflow does not submit ACH.
- The workflow does not file taxes.
- Draft payroll runs need review before any later approval workflow.
