# Employer Setup and Payroll Settings

ClearPath Payroll provides local software tools for entering employer payroll setup data and payroll items. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Who Can Use It

Use this page from a local prototype or private deployment with access to employer payroll setup records.

## What It Does

- Stores employer contact and address details in the local database.
- Stores FEIN values and displays a masked FEIN after save.
- Stores employer tax setup placeholders, including user-entered rate fields.
- Stores payroll items for earnings, deductions, taxes, reimbursements, and memo-only records.
- Does not call tax APIs, submit ACH, or submit tax filings.

## Steps

1. Open **Setup > Employer**.
2. Select **Add New Employer**.
3. Enter legal name, optional DBA, FEIN, contact, address, county, local tax locality placeholder, and active status.
4. Select **Save**.
5. Open **Setup > Payroll Settings**.
6. Select the company.
7. Enter employer tax setup placeholders and user-entered rate values.
8. Select **Save Payroll Settings**.
9. Select **Add New Payroll Item**.
10. Enter item code, name, type, calculation type, defaults, tax flags, dates, and notes.
11. Select **Save Item**.

## Required Permissions

Local application access with permission to edit employer setup records.

## Common Errors

- Missing required employer fields.
- Invalid email or state length.
- Plain account numbers entered in placeholder fields.
- Payroll item end date earlier than effective date.
- Duplicate payroll item code for the same company.

## Cautions

Verify with official agency records. Account numbers are masked where displayed. Do not enter full bank account numbers, routing numbers, or unrelated sensitive data into placeholder fields.
