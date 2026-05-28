# Import Employees

The employee import page lets testers import employee setup rows from a local CSV, tab-delimited, or Excel file after reviewing mappings and validation results.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Name Mapping

Map either:

- `FirstName` and `LastName`, or
- `FullName`

When `FullName` is used, ClearPath Payroll parses common formats such as `John Smith`, `John A Smith`, and `Smith, John`. If the name cannot be parsed confidently, the row is flagged for review before import.

## Required Fields

- `PayType`
- `SSNLast4` or `EmployeeNumber`
- Employee name using either `FirstName` plus `LastName`, or `FullName`

Hourly employees need `HourlyRate`. Salary employees need `AnnualSalary`.

## Steps

1. Open `/import/employees`.
2. Select a company.
3. Select a local employee file.
4. Review suggested mappings.
5. Map either first and last name, or full name.
6. Validate employees.
7. Fix any validation messages.
8. Confirm the import only after the preview is acceptable.

## Safety Notes

Do not import full SSNs, full bank account numbers, routing numbers, passwords, API keys, or live payroll data into the demo package.
