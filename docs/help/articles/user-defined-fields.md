# User-Defined Fields

## What this page does

User-Defined Fields lets testers create optional custom fields for company, employee, payroll run, payroll run employee, or check line records.

## When to use it

Use it when testing custom labels, notes, numbers, dates, currency, percent, boolean, or list values.

## Before you begin

Select a demo company. Decide what fake field name and code to test.

## Step-by-step instructions

1. Open `/setup/user-defined-fields`.
2. Select a company.
3. Add a field name and field code.
4. Choose where the field applies.
5. Choose the data type.
6. Set optional default/list values.
7. Save the field.
8. Open a related screen and confirm the field is available where expected.

## What to check

- Required field validation works.
- Field codes are unique for the company.
- Script or formula execution is not available.
- Placeholder formula roles do not run code.

## Common messages or errors

- Duplicate field code: choose a different code.
- Missing list options: add options for list fields.

## What this page does not do

It does not decide which fields are required for an employer and does not execute scripts.

## Privacy/safety notes

Do not store SSNs, bank numbers, API keys, or live payroll data in custom fields.

## Related articles

- [Payroll Field Templates](payroll-field-templates.md)
- [Employee Setup](employee-setup.md)
- [Creating a Payroll Run](creating-payroll-run.md)
