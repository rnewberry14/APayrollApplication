# Checks Import Admin Notes

The checks import workflow stages local file rows, validates totals, and creates Draft after-the-fact payroll runs after user confirmation.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Routes

- `/import/checks`
- `/import/employees-and-checks`

## Permissions

- Setup/import access
- Local database write access through the application
- Local read access to the selected import file

## Validation

The workflow checks:

- Required column mappings
- Company identifier match
- Employee identifier match for checks-only imports
- Net pay formula with rounding tolerance:
  `GrossPay - EmployeeTaxes - Deductions = NetPay`
- Direct deposit last four format
- Sensitive column names that imply full SSN, routing number, or full bank account number

## Result

Confirmed imports create:

- A Draft payroll run marked `After-the-fact payroll`
- Payroll run employee records
- Earning lines
- Deduction lines
- Employee tax lines
- Net pay lines
- Audit log entries

The workflow does not approve payroll.

## Cautions

- No ACH submission is performed.
- No tax filing is performed.
- No external provider call is performed.
- Imported data remains local to the configured database.
