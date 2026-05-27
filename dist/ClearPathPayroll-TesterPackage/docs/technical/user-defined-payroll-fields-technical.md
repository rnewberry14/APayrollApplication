# User-Defined Payroll Fields Technical Notes

ClearPath Payroll provides local software tools for payroll data entry and organization. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Route

- `/setup/user-defined-fields`

## Domain Model

`UserDefinedFieldDefinition` stores:

- Company scope.
- Field name and field code.
- Applies-to target.
- Data type.
- Required/default/list metadata.
- Calculation inclusion flag and calculation role.
- Active flag and timestamps.

`UserDefinedFieldValue` stores typed values for a definition and target record:

- Text, number, decimal, date, or boolean value.
- Entity type and entity id.
- Timestamps.

## Validation

- Field codes allow letters, numbers, underscores, and hyphens.
- List fields require list options.
- Script-like text, JavaScript URL text, and template expressions are rejected.
- Values are checked against the definition's target entity type and data type.
- List values are checked against configured list options.

## Persistence

EF Core maps definitions to `UserDefinedFieldDefinitions` and values to `UserDefinedFieldValues`. The migration `20260526133000_AddUserDefinedPayrollFields` creates both tables and unique indexes for field code and entity values.

## External Calls

No external provider calls, tax API calls, ACH submissions, telemetry, cloud storage, or hosted database dependency is added.

## Calculation Use

`IncludeInPayrollCalculation` and `CalculationRole` are metadata only for now. Formula support is a placeholder and no arbitrary code execution is allowed.
