# Printing Paychecks

## What this page does

The Paycheck page creates a printable demo paycheck for employees not paid by direct deposit.

## When to use it

Use it when reviewing check printing for non-direct-deposit demo employees.

## Before you begin

Create and calculate payroll for an employee whose payment method is not direct deposit.

## Step-by-step instructions

1. Open `/payroll/paycheck/{payrollRunEmployeeId}`.
2. Review company and employee names.
3. Review pay date, pay period, net pay, memo, and check number field.
4. Confirm demo/non-negotiable watermark appears in local demo mode.
5. Click `Print Paycheck` or use browser print.
6. Choose `Print to PDF` if testing digital output.

## What to check

- No full SSN appears.
- No bank account number appears.
- Check is printable.
- Demo watermark appears.

## Common messages or errors

- Direct deposit employee: paycheck printing is not available for that employee.
- Missing paycheck data: return to payroll run or preview.

## What this page does not do

It does not create bank positive-pay files, submit ACH, or imply bank approval.

## Privacy/safety notes

Printed checks are user-controlled demo documents.

## Related articles

- [Creating a Payroll Run](creating-payroll-run.md)
- [Printing Pay Stubs](printing-pay-stubs.md)
