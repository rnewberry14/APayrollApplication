# Approving Payroll

## What this page does

Payroll approval changes a valid demo payroll run to Approved status after confirmation.

## When to use it

Use it after reviewing payroll preview and resolving blocking errors.

## Before you begin

Open payroll preview for a valid demo payroll run.

## Step-by-step instructions

1. Open `/payroll/preview/{payrollRunId}`.
2. Review payroll totals and employee checks.
3. Confirm no blocking validation errors are shown.
4. Click `Approve Payroll`.
5. Read the confirmation message.
6. Confirm approval.
7. Confirm the page refreshes and shows Approved status.

## What to check

- Approval is disabled when blocking errors exist.
- Approved status appears after confirmation.
- Approved payroll cannot be casually recalculated.
- Audit information appears if visible.

## Common messages or errors

- Cannot approve because validation errors exist: correct errors first.
- Payroll already approved: no second approval is needed.

## What this page does not do

Approval does not submit direct deposit, submit ACH, or file taxes.

## Privacy/safety notes

Approve demo payroll only. Do not use this workflow for live payroll.

## Related articles

- [Payroll Preview](payroll-preview.md)
- [Fake Direct Deposit Submission](fake-direct-deposit-submission.md)
- [Payroll Register](payroll-register.md)
