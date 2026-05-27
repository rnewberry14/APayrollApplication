# Printable Paycheck

Date: 2026-05-26

## Added

- Printable paycheck route at `/payroll/paycheck/{payrollRunEmployeeId}`.
- `PaycheckPrintService` for generated paycheck data.
- Browser print button and print CSS.
- Check number tracking placeholder.
- Net pay amount in words.
- Local Demo Mode non-negotiable/demo watermark.
- Tests and documentation.

## Safety Notes

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. Printed checks are user-controlled documents. This feature does not create real bank-positive-pay files, submit ACH, transmit bank data, imply bank approval, or print full SSNs or bank account numbers.
