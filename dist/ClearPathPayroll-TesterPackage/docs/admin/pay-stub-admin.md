# Pay Stub Printing Administration

ClearPath Payroll stores payroll data locally and prepares printable pay stub views. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Feature Scope

- Current route: `/payroll/paystub/{payrollRunEmployeeId}`.
- Legacy route remains available: `/paystub/{payrollRunId}/{employeeId}`.
- Uses `PayStubService` and existing payroll line items.
- Includes configurable print display settings on the page.

## Required Permissions

Payroll access to the local application and selected payroll run employee record.

## Configuration Notes

The display settings are page-level print options:

- Show hours.
- Show rates.
- Show YTD.
- Show employer taxes.
- Show employee address.
- Show accrual placeholders.
- State-required-fields checklist placeholder.

## Security Cautions

The page displays SSN last 4 only and direct deposit last 4 only. It does not display full bank account numbers.

## Distribution Caution

Review applicable pay statement requirements before distribution. The checklist area is a placeholder for local review tracking.
