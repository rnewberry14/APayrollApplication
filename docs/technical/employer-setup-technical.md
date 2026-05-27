# Employer Setup Technical Notes

ClearPath Payroll provides local software tools for payroll data entry and storage. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Routes

- `/setup/employer`
- `/setup/employer/payroll-settings`

## Domain Model

`Company` stores employer contact, address, locality placeholder, and employer tax setup placeholder fields. Account placeholder validation rejects plain numeric values that resemble account numbers.

`PayrollItem` stores company-specific payroll item configuration:

- Item code and name.
- Item type.
- Calculation type.
- Default amount or user-entered rate.
- Taxability flags.
- Effective and end dates.
- Notes and timestamps.

## Services

- `CompanyService` loads and saves employer setup data, masks FEIN values, and masks account placeholders for display.
- `PayrollItemService` loads and saves payroll items with data annotation validation.

## Persistence

EF Core maps employer setup fields to `Companies` and payroll items to `PayrollItems`. The migration `20260526130500_AddEmployerSetupAndPayrollItems` adds the employer setup columns and payroll item table.

## External Calls

No tax API, ACH API, telemetry, cloud storage, or hosted database dependency is added by this feature.

## Security Cautions

FEIN values are masked in list displays. Placeholder account fields are masked where displayed. Full bank account numbers and routing numbers are not expected in these fields.
