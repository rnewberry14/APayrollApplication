# Optional Payroll Field Templates Administration

ClearPath Payroll stores activated field templates as local user-defined field definitions. The software does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Feature Scope

- Setup route: `/setup/field-templates`.
- Uses the existing `UserDefinedFieldDefinition` system.
- Templates are catalog entries until explicitly activated.
- Activated templates can be edited from `/setup/user-defined-fields`.

## Required Permissions

Administrative setup access to the local application and selected company.

## Administration Steps

1. Open **Setup > Field Templates**.
2. Select a company.
3. Review the common optional payroll field template catalog.
4. Activate only the templates the company wants to use.
5. Open **User-Defined Fields** to edit activated field names or codes.

## Security Cautions

Do not store full SSNs, full bank account numbers, routing numbers, API keys, or provider credentials in activated fields.

## Payroll And Tax Cautions

Templates are optional local setup aids. They are not statements about payroll, tax, ACH, or reporting requirements.
