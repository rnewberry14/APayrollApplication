# Employee Setup

## What this page does

Employee Setup lets testers add and edit demo employee payroll profile information.

## When to use it

Use it when testing employee demographics, employment settings, pay rates, tax setup fields, direct deposit placeholders, custom fields, and notes.

## Before you begin

Create or select a demo company. Use fake employee data only.

## Step-by-step instructions

1. Open `/setup/employees`.
2. Select a company.
3. Click `Add New Employee`.
4. Complete the Demographics tab using fake data.
5. Complete Employment and Pay tabs.
6. Add Federal W-4 and State Tax placeholder values.
7. Add direct deposit token/last-four demo values if needed.
8. Add user-defined field values if fields exist.
9. Save the employee.

## What to check

- Full SSN is not requested.
- Only SSN last four is shown.
- Full bank account and routing numbers are not displayed.
- Validation messages are clear.

## Common messages or errors

- Required field missing: fill in the highlighted field.
- Too many direct deposit accounts: keep the list to five or fewer.
- Invalid last four: use four digits only.

## What this page does not do

It does not transmit employee data, verify bank accounts, or provide tax/payroll advice.

## Privacy/safety notes

Do not enter real SSNs, bank account numbers, routing numbers, EINs, or live payroll data.

## Related articles

- [Direct Deposit Setup](direct-deposit-setup.md)
- [User-Defined Fields](user-defined-fields.md)
- [Creating a Payroll Run](creating-payroll-run.md)
