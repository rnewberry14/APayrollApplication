# Importing Payroll Checks

## What this page does

Payroll check import loads user-provided demo check data from local files for after-the-fact payroll review.

## When to use it

Use it to test checks-only or employees-and-checks import workflows.

## Before you begin

Prepare a fake/demo CSV, tab-delimited, or Excel file.

## Step-by-step instructions

1. Open `/import/checks` for checks only or `/import/employees-and-checks` for employees and checks.
2. Select a local file.
3. Map columns.
4. Review total validation.
5. Review mismatches and warnings.
6. Preview rows.
7. Confirm import only after review.
8. Open payroll preview or register to inspect imported data.

## What to check

- Gross pay minus taxes and deductions matches net pay within rounding tolerance.
- Mismatches are flagged.
- Imported payroll is not auto-approved.

## Common messages or errors

- Missing employee identifier: map the employee identifier column.
- Net pay mismatch: review imported amounts.

## What this page does not do

It does not submit ACH, file taxes, or approve imported payroll automatically.

## Privacy/safety notes

Imported data is user-provided demo data and must be reviewed.

## Related articles

- [Importing Employees](importing-employees.md)
- [Payroll Preview](payroll-preview.md)
- [Payroll Register](payroll-register.md)
