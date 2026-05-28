# Import Employees Admin Notes

The employee import workflow stages local file rows, validates mappings, and creates employee records only after user confirmation.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Name Mapping Rule

Employee name mapping is valid when either `FirstName` and `LastName` are mapped, or `FullName` is mapped. If all three are mapped, `FirstName` and `LastName` are treated as the primary employee name fields.

## Duplicate Checks

The import blocks duplicates by:

- `EmployeeNumber`
- Employee name plus `SSNLast4`

`FullName` is parsed into first and last name before duplicate checks.

## Admin Review Points

- Confirm the selected company is correct.
- Confirm no full SSN columns are mapped.
- Confirm no full bank account or routing number columns are mapped.
- Review rows flagged for uncertain full-name parsing.
- Confirm the import only after validation passes.
