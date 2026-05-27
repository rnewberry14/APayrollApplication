# Printable Paycheck Troubleshooting

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. This document covers software behavior only.

## Common Errors

- **Printable paycheck is not available**: Confirm the payroll run employee ID in the route.
- **Direct deposit block**: The employee check has a `DirectDeposit` net pay line.
- **Missing employee data**: Confirm the payroll run employee is linked to an employee.
- **Missing company data**: Confirm the payroll run is linked to a company.
- **Print button does nothing**: Confirm browser pop-up or print dialog settings allow `window.print`.

## Troubleshooting Steps

1. Confirm payroll has been calculated.
2. Confirm the employee is not paid by direct deposit for this check.
3. Confirm net pay is greater than or equal to zero.
4. Open `/payroll/paycheck/{payrollRunEmployeeId}`.
5. Enter a check number tracking placeholder.
6. Select **Print Paycheck**.

## Safety Cautions

Printed checks are user-controlled documents. This feature does not create real bank-positive-pay files, submit ACH, transmit bank data, or imply bank approval.
