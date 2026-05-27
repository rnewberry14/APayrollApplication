# User-Defined Payroll Fields Administration

ClearPath Payroll stores user-defined payroll field setup in the local database. The application organizes user-entered data and does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Feature Scope

- Setup route: `/setup/user-defined-fields`.
- Definition model: `UserDefinedFieldDefinition`.
- Value model: `UserDefinedFieldValue`.
- Supported target records: company, employee, payroll run, payroll run employee, and check line.
- Formula support is metadata only for now.

## Required Permissions

Administrative setup access to the local application and the selected company.

## Administration Steps

1. Open the setup page.
2. Select a company.
3. Create an optional custom field.
4. Choose data type and applies-to scope.
5. Add list options for list fields.
6. Keep calculation role as informational unless a later local calculation workflow uses the field.
7. Save the definition.

## Security Cautions

Do not store full SSNs, full bank account numbers, routing numbers, API keys, or provider credentials in user-defined fields. The setup page blocks script-like text and does not execute formulas.

## Payroll And Tax Cautions

User-defined fields do not identify legally required fields. Verify with official agency records and qualified professionals where appropriate.
