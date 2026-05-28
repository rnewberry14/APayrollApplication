# Employer Setup

## What this page does

Employer Setup stores demo employer details such as name, address, payroll contact, user-entered FEIN, and active status.

## When to use it

Use it to review, add, or edit a demo employer before creating payroll runs.

## Before you begin

Create demo data or prepare fake employer information.

## Step-by-step instructions

1. Open `/setup/employer`.
2. Select an existing employer or add a new one.
3. Enter legal name, DBA name, contact, phone, email, and address.
4. Enter fake FEIN-style test data only.
5. Save the employer.
6. Confirm the FEIN remains the user-entered value after save.
7. Open `/setup/employer/payroll-settings` for tax settings and payroll items.

## What to check

- Required fields show validation messages.
- Saved employer appears in the list.
- FEIN is not replaced with demo placeholder text.
- The page uses neutral wording.

## Common messages or errors

- Required field missing: complete the highlighted field.
- Duplicate or invalid value: use a different fake value.

## What this page does not do

It does not verify employer tax IDs, determine tax rates, or provide advice.

## Privacy/safety notes

Do not enter real EINs, tax account numbers, API keys, or live payroll data.

## Related articles

- [Local Demo Mode Explained](local-demo-mode-explained.md)
- [Payroll Field Templates](payroll-field-templates.md)
- [Tax Liability Report](tax-liability-report.md)
