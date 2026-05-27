# Importing Employees

## What this page does

Employee import loads demo employee records from local CSV, tab-delimited, or Excel files using mapping, validation, preview, and confirmation.

## When to use it

Use it to test importing employees without typing each employee manually.

## Before you begin

Prepare a fake/demo file. Do not include full SSNs or real bank data.

## Step-by-step instructions

1. Open `/import/employees`.
2. Select a local CSV, tab-delimited, or Excel file.
3. Map file columns to employee fields.
4. Review validation messages.
5. Review the preview.
6. Confirm import only after review.
7. Open `/setup/employees` to check imported employees.

## What to check

- Required columns are mapped.
- Duplicate employees are flagged.
- Full SSNs are blocked.
- Preview appears before save.

## Common messages or errors

- Required column missing: map FirstName, LastName, PayType, and SSNLast4 or EmployeeNumber.
- Duplicate employee: review existing employee list.

## What this page does not do

It does not upload files to cloud services or import real sensitive data.

## Privacy/safety notes

Use local fake files only. Do not import full SSNs or full bank account numbers.

## Related articles

- [Employee Setup](employee-setup.md)
- [Importing Payroll Checks](importing-payroll-checks.md)
- [How to Report Feedback](how-to-report-feedback.md)
