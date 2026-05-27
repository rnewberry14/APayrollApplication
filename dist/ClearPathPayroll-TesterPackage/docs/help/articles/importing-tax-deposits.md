# Importing Tax Deposits

## What this page does

Tax deposit import saves user-entered demo tax deposit records for review and reporting placeholders.

## When to use it

Use it when testing historical tax deposit records in the tax liability report.

## Before you begin

Prepare a fake/demo local file with deposit date, tax period, tax type, agency, amount, and related details.

## Step-by-step instructions

1. Open `/import/tax-deposits`.
2. Select a CSV, tab-delimited, or Excel file.
3. Map columns.
4. Review validation messages.
5. Preview the deposits.
6. Confirm save after review.
7. Open `/reports/tax-liability`.

## What to check

- Required fields are validated.
- Records are marked as user-entered deposit records.
- Tax liability report can show payment status placeholders.

## Common messages or errors

- Missing amount or date: complete the mapped field.
- Invalid date: correct date format in the source file.

## What this page does not do

It does not submit payments, call IRS/state systems, or verify deposits.

## Privacy/safety notes

Do not import real confirmation numbers or live tax payment data.

## Related articles

- [Tax Liability Report](tax-liability-report.md)
- [Importing Payroll Checks](importing-payroll-checks.md)
