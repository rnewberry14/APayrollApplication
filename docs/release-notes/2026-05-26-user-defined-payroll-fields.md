# User-Defined Payroll Fields

Date: 2026-05-26

## Added

- User-defined payroll field setup page at `/setup/user-defined-fields`.
- `UserDefinedFieldDefinition` and `UserDefinedFieldValue` models.
- EF Core mapping and migration for field definitions and typed values.
- `UserDefinedFieldService` for local definition and value persistence.
- Navigation link under Setup.
- Validation for unsafe script-like text, list options, field codes, and typed values.
- Documentation and unit tests.

## Safety Notes

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. Optional custom fields are local records. Formula support is a placeholder only and no arbitrary code is executed.
