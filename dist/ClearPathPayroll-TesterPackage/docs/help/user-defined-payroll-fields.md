# User-Defined Payroll Fields

ClearPath Payroll provides local software tools for payroll data entry and organization. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## What This Feature Does

User-defined payroll fields let users create an optional custom field for company, employee, payroll run, payroll check, or check line records. The field definition controls the label, code, data type, default value, list choices, and whether the field may be included in payroll calculations later.

Formula support is a placeholder only. The feature does not run scripts or formulas.

## Who Can Use It

Users with access to setup pages in the local ClearPath Payroll application.

## Steps

1. Open **Setup > User-Defined Fields**.
2. Select a company.
3. Select **Add Field**.
4. Enter field name and field code.
5. Select where the field applies.
6. Select a data type.
7. Optional: enter a default value or list options.
8. Optional: mark the field active or required.
9. Optional: select a calculation role for later payroll calculation use.
10. Select **Save**.

## Required Permissions

Local setup access for the selected company.

## Common Errors

- Field name or field code is blank.
- Field code contains characters other than letters, numbers, underscores, or hyphens.
- A list field has no list options.
- Text contains scripts, templates, or executable expressions.
- Field code already exists for the selected company.

## Cautions

Optional custom field definitions are local configuration records. Verify payroll data using appropriate records and official sources. Do not enter SSNs, full bank account numbers, routing numbers, API keys, or other sensitive secrets in custom fields.
