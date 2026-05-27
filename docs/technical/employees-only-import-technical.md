# Employees Only Import Technical Notes

The Employees Only Import workflow is implemented at `/import/employees` and uses the generic import parser and staging framework.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Components

- `EmployeesOnlyImport.razor`: company selection, local file selection, mapping, preview, validation, and confirmation UI.
- `EmployeesOnlyImportService`: employee-specific validation and confirmed local employee creation.
- `ImportService`: generic parsing, staging, validation status, and batch confirmation.
- `CsvImportFileParser`, `TabDelimitedImportFileParser`, and `ExcelImportFileParser`: local file parsers.

## Validation

The service validates:

- Required mappings for `FirstName`, `LastName`, and `PayType`.
- Either `SSNLast4` or `EmployeeNumber`.
- `SSNLast4` format when mapped.
- `DirectDepositLast4` format when mapped.
- Pay type values of `Hourly` or `Salary`.
- Hourly rate for hourly employees.
- Annual salary for salary employees.
- Duplicate employee number in the selected company and file.
- Duplicate first name, last name, and SSN last four in the selected company and file.
- Mapped sensitive columns such as full SSN, routing number, or bank account number.

## Persistence

Validation creates a local `ImportBatch`, `ImportRow`, `ImportMapping`, and any `ImportError` records. Employee records are created only after confirmation succeeds.

Missing optional fields that are required by the current employee model use neutral local placeholders and can be edited after import.

## External Systems

This workflow does not call cloud storage, tax APIs, ACH providers, telemetry endpoints, or hosted database services.
