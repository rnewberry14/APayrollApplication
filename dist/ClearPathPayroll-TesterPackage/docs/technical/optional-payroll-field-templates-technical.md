# Optional Payroll Field Templates Technical Notes

ClearPath Payroll provides local software tools for payroll data entry and organization. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Route

- `/setup/field-templates`

## Catalog

`PayrollFieldTemplateCatalog` exposes the common optional payroll field template list. Catalog entries are in-memory metadata and do not write database records.

## Activation

`UserDefinedFieldService.ActivateTemplateAsync` creates a `UserDefinedFieldDefinition` for the selected company. Duplicate field codes are rejected. Templates are not activated automatically.

## Editing

Activated templates are normal user-defined field definitions. Field name and field code edits happen on `/setup/user-defined-fields`.

## Persistence

No new database schema is required. Activated templates use the existing `UserDefinedFieldDefinitions` table.

## External Calls

No external provider calls, tax API calls, ACH submissions, telemetry, cloud storage, or hosted database dependency is added.

## Calculation Use

Some templates set `IncludeInPayrollCalculation` and `CalculationRole` metadata for later local workflows. No calculation logic changes are included.
