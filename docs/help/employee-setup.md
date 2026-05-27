# Employee Setup - User Guide

## What This Feature Does

Employee Setup lets users select a company, view employees, and add or edit local employee payroll profile records. It includes demographics, employment, pay, federal W-4, state tax, direct deposit token setup, user-defined fields, and notes.

## Who Can Use It

Users who maintain local employee payroll setup records.

## Required Permissions

- View companies and employees.
- Create and edit employee payroll profiles.
- Create and edit tokenized direct deposit setup records.

## Step-By-Step Use

1. Open `/setup/employees`.
2. Select a company.
3. Select **Add New Employee** or **Edit Employee**.
4. Complete each tab: Demographics, Employment, Pay, Federal W-4, State Tax, Direct Deposit, User-Defined Fields, and Notes.
5. Select **Save** to store the employee locally.
6. Select **Cancel** to exit editing without saving.

## Common Errors

| Error | Cause | Resolution |
| --- | --- | --- |
| Hourly rate is required | Pay type is Hourly | Enter an hourly rate greater than zero |
| Annual salary is required | Pay type is Salary | Enter an annual salary greater than zero |
| Token validation fails | A raw routing or account number was entered | Enter token placeholder values and last four only |
| Direct deposit validation fails | More than five active accounts or invalid split setup | Review active accounts, priority/order, and remainder account |

## Cautions

- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
- Do not enter full SSNs or full bank account numbers.
- This page does not transmit data externally, submit real ACH, or submit tax filings.
