# Checks Import Technical Notes

The checks import workflow adds specialized after-the-fact payroll import behavior on top of the generic import framework.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Components

- `ChecksImport.razor`: two-route page for `/import/checks` and `/import/employees-and-checks`.
- `ChecksImportService`: validation and confirmed local payroll-run creation.
- `ImportService`: parser selection, batch staging, and batch confirmation.
- `PayrollRun.PayrollMode`: stores `After-the-fact payroll` for imported check runs.

## Validation

The service validates required mappings, row parsing, company matching, employee matching, sensitive columns, direct deposit last four, and net pay totals. Net pay validation uses a one-cent rounding tolerance.

## Confirmation

Confirmation groups valid rows by company, pay date, and pay period. Each group creates a Draft payroll run and child line records:

- `PayrollRunEmployee`
- `EarningLine`
- `DeductionLine`
- `TaxLine`
- `NetPayLine`
- `AuditLogEntry`

The workflow does not approve payroll, submit ACH, file taxes, or call external provider services.

## Employees And Checks Mode

When `/import/employees-and-checks` is used and an employee identifier is not found, the service creates a minimal local employee placeholder using the employee identifier. The placeholder stores SSN last four as `0000` and can be edited later.
