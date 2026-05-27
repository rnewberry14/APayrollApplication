# Check Calculation Preview Administration

ClearPath Payroll stores check calculation preview results in the local database. The software does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Feature Scope

- Route: `/payroll/check-calculation/{payrollRunId}`.
- Editable check inputs for draft or calculated payroll runs.
- Recalculate one check or all checks.
- Save earning, deduction, tax, net pay, and employer tax line items.
- Add audit log entries for recalculation events.

## Required Permissions

Administrative payroll access to the local application and selected payroll run.

## Configuration Notes

If sandbox tax lookup is enabled, the configured sandbox tax calculation service can be used. Otherwise, the fake tax calculation service is used for local preview.

## Operational Cautions

- Approved, submitted, and completed payroll runs are not recalculated from this page.
- This page does not approve payroll.
- This page does not submit ACH.
- This page does not submit tax filings.
- Full SSNs and full bank account numbers are not displayed.
