# Employee Payroll Profile - Admin Guide

## What This Feature Does

The employee payroll profile model expands local employee setup storage for payroll workflows. It keeps employee payroll setup data in the local database and supports tokenized direct deposit setup for up to five active accounts.

## Who Can Use It

Payroll administrators maintaining local employee setup data.

## Required Permissions

- Employee setup read/write access.
- Tokenized direct deposit setup read/write access.

## Admin Steps

1. Confirm employee demographic and employment fields are complete.
2. Confirm pay type has the matching pay rate field.
3. Confirm W-4 and state tax setup fields are entered from user-provided records.
4. Confirm direct deposit records use token placeholders and last four only.
5. Confirm no more than five active direct deposit records exist per employee.

## Common Errors

- Hourly employee missing hourly rate.
- Salary employee missing annual salary.
- Direct deposit percentage over 100.
- More than one active remainder account.

## Cautions

- Do not store full SSNs, full bank account numbers, routing numbers, or provider secrets in plain text.
- Do not use employee setup data as legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
