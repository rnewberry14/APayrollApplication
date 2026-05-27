# Fake Direct Deposit Submission

## What this page does

Fake Direct Deposit Submission creates a sandbox/fake direct deposit batch for an approved demo payroll run.

## When to use it

Use it only after payroll is approved and local demo mode allows fake ACH.

## Before you begin

Approve a demo payroll run and confirm employees have fake verified direct deposit token records.

## Step-by-step instructions

1. Open payroll preview for an Approved payroll run.
2. Confirm the `Submit Direct Deposit` button is visible.
3. Click `Submit Direct Deposit`.
4. Review the confirmation message.
5. Confirm submission.
6. Review fake batch status and fake external batch reference.
7. Try submitting again and confirm duplicate submission is blocked.

## What to check

- The button appears only for Approved payroll.
- Fake batch reference appears after submission.
- Duplicate submission is blocked.
- Only account last four is shown.

## Common messages or errors

- Missing or unverified bank account: update fake direct deposit setup.
- Already submitted: duplicate submission is blocked.

## What this page does not do

It does not submit real ACH or transmit real bank data.

## Privacy/safety notes

Do not enter or transmit real bank account numbers, routing numbers, passwords, or API keys.

## Related articles

- [Direct Deposit Setup](direct-deposit-setup.md)
- [Approving Payroll](approving-payroll.md)
- [Payroll Preview](payroll-preview.md)
