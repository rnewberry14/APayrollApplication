# Calculating Checks Before Approval

## What this page does

The Check Calculation page lets testers review and recalculate employee checks before payroll approval.

## When to use it

Use it after creating a draft payroll run and before approving payroll.

## Before you begin

Create a draft payroll run with demo employees.

## Step-by-step instructions

1. Open `/payroll/check-calculation/{payrollRunId}` from the payroll workflow.
2. Review each employee check.
3. Enter or edit regular hours.
4. Enter overtime placeholder values if needed.
5. Add additional earnings, deductions, reimbursements, and notes.
6. Click `Recalculate Check` for one employee.
7. Click `Recalculate All` for the whole run.
8. Review warnings and blocking errors.

## What to check

- Gross pay, deductions, taxes, and net pay update.
- Negative net pay is blocked.
- Minimum wage warning appears when applicable.
- Approved payroll runs are not recalculated casually.

## Common messages or errors

- Blocking validation error: correct the check before approval.
- Payroll already approved: use an approved-run correction workflow when available.

## What this page does not do

It does not submit ACH, file taxes, or approve payroll unless a separate approval confirmation is used.

## Privacy/safety notes

Calculations are for demo review only and are not legal, tax, financial, accounting, payroll compliance, or licensed professional advice.

## Related articles

- [Creating a Payroll Run](creating-payroll-run.md)
- [Payroll Preview](payroll-preview.md)
- [Approving Payroll](approving-payroll.md)
