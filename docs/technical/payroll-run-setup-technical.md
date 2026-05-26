# Payroll Run Setup - Technical Notes

## Overview

Payroll Run Setup is a Blazor page at `/payroll/create`. It creates a draft payroll run for local prototype use.

## Implementation Details

- Page: `src/ClearPathPayroll/Components/Pages/PayrollRunCreate.razor`
- Service: `PayrollService.CreateDraftPayrollRunAsync`
- Route: `/payroll/create`
- Redirect after save: `/payroll/preview/{PayrollRunId}`

The service creates:

- `PayrollRun` in `Draft` status.
- `PayrollRunEmployee` records for selected active employees.
- `EarningLine` records for regular earnings and optional manual gross pay adjustments.
- `AuditLogEntry` with event type `PayrollRunCreated`.

The service does not create tax lines, employer tax lines, direct deposit batches, direct deposit items, or ACH records.

## Who Can Use It

Users with payroll run creation access in Development or Local Prototype Mode.

## Required Permissions

- Read active companies.
- Read active pay schedules.
- Read active employees.
- Create payroll runs.
- Create payroll run employees.

## Validation

- Company ID must be selected.
- Pay schedule ID must be selected.
- At least one employee must be selected.
- Selected employees must be active and belong to the selected company.
- Pay schedule must be active and belong to the selected company.
- Regular hours and manual gross pay adjustments cannot be negative.

## Common Errors and Troubleshooting

- Missing schedule: verify `PaySchedule.IsActive` and `CompanyId`.
- Missing employee: verify `Employee.EmploymentStatus` and `CompanyId`.
- Unexpected gross pay: review `GrossPayCalculationService` and entered regular hours.
- No tests run from solution: run the test project directly if the solution omits test projects.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not submit ACH.
- Do not submit tax filings.
- Do not call production APIs.
- Do not log SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets.
