# Direct Deposit Setup

## What this page does

Direct Deposit Setup stores fake/sandbox bank token information for demo employee payment testing.

## When to use it

Use it when testing employee setup or fake direct deposit submission.

## Before you begin

Open an employee in `/setup/employees`. Use token placeholders and last-four demo values only.

## Step-by-step instructions

1. Open `/setup/employees`.
2. Select a company and employee.
3. Open the Direct Deposit tab.
4. Add up to five demo accounts.
5. Enter bank name, account type, token placeholders, last four, deposit type, and active status.
6. Save the employee.
7. Use payroll preview to test fake direct deposit later.

## What to check

- Full account number is not requested.
- Routing number is not displayed as a real value.
- Last four is the only visible account identifier.
- Verification status is fake/sandbox.

## Common messages or errors

- Missing fake token: enter a placeholder token.
- Last four invalid: use four digits.
- More than five accounts: remove extra accounts.

## What this page does not do

It does not submit real ACH, verify real bank data, or store full bank account numbers.

## Privacy/safety notes

Do not enter real bank account numbers, routing numbers, passwords, or API keys.

## Related articles

- [Employee Setup](employee-setup.md)
- [Fake Direct Deposit Submission](fake-direct-deposit-submission.md)
- [Payroll Preview](payroll-preview.md)
