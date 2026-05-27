# Check Calculation Preview Technical Notes

ClearPath Payroll provides local software tools for payroll calculation, reporting, and data organization. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Route

- `/payroll/check-calculation/{payrollRunId}`

## Service

`CheckCalculationPreviewService` loads payroll runs with selected employee checks, builds editable check inputs, recalculates one check or all checks, saves line items, updates totals, and writes audit logs.

## Saved Line Items

- `EarningLines` for regular pay, overtime placeholder pay, additional earnings, reimbursements, and notes.
- `DeductionLines` for pre-tax and manual deduction amounts.
- `TaxLines` for employee tax totals.
- `NetPayLines` for preview net pay.
- `EmployerTaxLines` for employer tax totals.

## Tax Calculation Selection

The configured tax calculation service is used only when sandbox lookup is enabled. Otherwise the fake tax calculation service is used for local preview.

## Recalculation Lock

Draft and Calculated runs can be recalculated. Approved, Submitted, and Completed runs are blocked from recalculation on this page.

## External Calls

This feature does not submit ACH, submit tax filings, add telemetry, add cloud storage, or change production provider configuration.
