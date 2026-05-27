# Printable Paycheck Administration

ClearPath Payroll stores payroll data locally and provides printable document views. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Feature Scope

- Route: `/payroll/paycheck/{payrollRunEmployeeId}`.
- Service: `PaycheckPrintService`.
- Prints only non-direct-deposit employee checks.
- Uses existing payroll run, company, employee, and net pay line records.

## Required Permissions

Administrative payroll access to the local application.

## Administration Steps

1. Confirm payroll has been calculated.
2. Confirm the payroll run employee has a non-DirectDeposit payment method.
3. Open the paycheck route.
4. Enter the check number tracking placeholder.
5. Print from the browser.

## Security Cautions

The printable page does not show full SSNs, routing numbers, or bank account numbers.

## Payroll And Banking Cautions

Printed checks are user-controlled documents. This feature does not create real bank-positive-pay files, submit ACH, transmit bank data, or imply bank approval.
