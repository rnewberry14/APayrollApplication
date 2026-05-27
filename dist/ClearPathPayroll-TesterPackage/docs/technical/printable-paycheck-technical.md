# Printable Paycheck Technical Notes

ClearPath Payroll provides local software tools for payroll calculation, reporting, and document preparation. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Route

- `/payroll/paycheck/{payrollRunEmployeeId}`

## Service

`PaycheckPrintService` loads `PayrollRunEmployee` with related employee, payroll run, company, and net pay line data. The service blocks printable paycheck generation when any net pay line has `PaymentMethod` equal to `DirectDeposit`.

## Printable Data

The printable model includes company name and address, employee name and address, pay date, check number tracking placeholder, net pay amount, net pay in words, memo line, and pay period.

## Print Styling

The Razor page includes print CSS using `@media print` and hides non-print controls. Local Demo Mode displays a non-negotiable/demo watermark.

## Security And Banking Limits

The page does not print full SSNs, routing numbers, or bank account numbers. This feature does not create real bank-positive-pay files, submit ACH, transmit bank data, or imply bank approval.
