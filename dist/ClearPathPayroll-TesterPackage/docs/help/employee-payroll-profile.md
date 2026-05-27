# Employee Payroll Profile - User Guide

## What This Feature Does

The employee payroll profile stores local employee setup data needed for payroll entry and review. It includes demographics, employment settings, pay defaults, federal W-4 fields, state tax fields, direct deposit token records, user-defined payroll fields, notes, and timestamps.

## Who Can Use It

Users who maintain employee setup records in the local ClearPath Payroll database.

## Required Permissions

- View employee setup records.
- Create or edit employee setup records.
- Create or edit tokenized direct deposit setup records.

## Step-By-Step Use

1. Add demographic information such as name, address, contact details, and SSN last four.
2. Add employment information such as employee number, status, worker type, department, job title, and work location placeholder.
3. Add pay information such as hourly rate or annual salary, default hours, earning code, and tipped employee placeholders.
4. Add federal W-4 fields from user-entered setup data.
5. Add state tax setup fields from user-entered setup data.
6. Add up to five active direct deposit accounts using token placeholders and last four only.
7. Add user-defined payroll fields and payroll notes as needed.

## Common Errors

| Error | Cause | Resolution |
| --- | --- | --- |
| SSN validation fails | More than last four was entered | Store only SSN last four |
| Bank token validation fails | Raw routing or account number was entered | Use token placeholder fields |
| Direct deposit validation fails | More than five active accounts or invalid deposit split | Review account order and deposit type |

## Cautions

- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
- Do not store full SSNs or full bank account numbers in plain text.
- Direct deposit fields are setup records only; they do not submit real ACH.
