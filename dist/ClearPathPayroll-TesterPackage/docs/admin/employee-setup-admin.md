# Employee Setup - Admin Guide

## What This Feature Does

Employee Setup provides a local-first screen for creating and editing employee payroll profile data. It saves employee records, direct deposit token setup, and user-defined field values in the local database.

## Who Can Use It

Payroll administrators or local prototype users who maintain employee setup records.

## Required Permissions

- Employee read/write access.
- Company read access.
- Direct deposit token setup read/write access.

## Admin Steps

1. Confirm the company exists.
2. Open `/setup/employees`.
3. Select the company.
4. Add or edit employee data by tab.
5. Confirm direct deposit fields use token placeholders and last four only.
6. Save the employee record.

## Common Errors

- More than five active direct deposit accounts.
- Raw numeric routing or account token values.
- Missing required pay rate for the selected pay type.
- Invalid SSN last four format.

## Cautions

- Do not store full SSNs or full bank account numbers in plain text.
- Do not submit real ACH or tax filings from setup.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
