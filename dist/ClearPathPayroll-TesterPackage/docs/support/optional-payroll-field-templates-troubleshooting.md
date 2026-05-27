# Optional Payroll Field Templates Troubleshooting

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. This document covers software behavior only.

## Common Errors

- **No templates shown as activated**: Templates remain inactive until the Activate button is selected.
- **Activate button is disabled**: The template is already active for the selected company.
- **Template will not activate**: Confirm a company is selected and the field code does not conflict with an existing user-defined field.
- **Need to rename a field**: Open **Setup > User-Defined Fields** and edit the activated field.

## Troubleshooting Steps

1. Select a company on the template page.
2. Check the Status column for each template.
3. Activate a template one at a time.
4. Use **Edit Activated Fields** for name or code changes.

## Security Cautions

Do not enter full SSNs, full bank account numbers, routing numbers, API keys, or secrets in activated custom fields.

## Payroll, Tax, And ACH Cautions

Templates do not submit ACH, submit tax filings, call production APIs, or identify required payroll fields.
