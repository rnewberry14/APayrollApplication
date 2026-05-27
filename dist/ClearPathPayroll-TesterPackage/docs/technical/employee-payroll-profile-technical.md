# Employee Payroll Profile - Technical Notes

## Overview

The employee payroll profile expands `Employee`, extends `EmployeeBankAccount`, and adds `EmployeePayrollField` for user-defined local payroll setup data.

## Model Changes

- `Employee`
  - Demographic fields
  - Employment fields
  - Pay defaults
  - Federal W-4 setup fields
  - State tax setup fields
  - Payroll notes and timestamps
- `EmployeeBankAccount`
  - Priority/order
  - Tokenized routing/account fields
  - Last four only
  - Deposit type, amount, percent, remainder flag
  - Verification and prenote placeholders
- `EmployeePayrollField`
  - User-defined payroll setup key/value records

## Validation

- Full SSN placeholder must be empty.
- Hourly employees require hourly rate.
- Salary employees require annual salary.
- Direct deposit token fields reject plain numeric routing/account values.
- Active direct deposit account collection supports no more than five active records.
- Active remainder account count is limited to one.

## Database

Migration: `20260526123000_AddEmployeePayrollProfileModels`

## Cautions

- Do not add plain-text SSN or bank account storage.
- Do not submit real ACH or tax filings from these models.
- Do not add advice or compliance guarantee wording.
