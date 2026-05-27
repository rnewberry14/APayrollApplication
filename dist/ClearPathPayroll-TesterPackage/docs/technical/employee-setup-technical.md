# Employee Setup - Technical Notes

## Overview

Employee Setup is implemented at `/setup/employees` in `EmployeeSetup.razor`. It uses `EmployeeService` and the existing employee payroll profile models.

## Components

- Page: `src/ClearPathPayroll/Components/Pages/EmployeeSetup.razor`
- Service: `EmployeeService`
- Models: `Employee`, `EmployeeBankAccount`, `EmployeePayrollField`
- Validation helper: `EmployeePayrollProfileValidator`

## Behavior

- Company selection loads employees for that company.
- Add New Employee creates a local draft model.
- Edit Employee loads the selected employee profile.
- Save validates data and persists employee, direct deposit token records, and user-defined fields.
- Direct deposit setup is limited to five active accounts.
- Full SSNs and full bank account numbers are not requested or displayed.

## Tests

- `EmployeeSetupPageTests`
- `EmployeeSetupServiceTests`
- `EmployeePayrollProfileValidationTests`

## Cautions

- Do not add external transmissions to this page.
- Do not add real ACH or tax filing calls.
- Do not store full SSNs or full bank account numbers.
- Do not add advice or compliance guarantee wording.
