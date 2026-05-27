# Employees Only Import Admin Notes

The Employees Only Import workflow uses the generic import framework to parse, stage, validate, preview, and confirm local employee imports.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Required Permissions

- Access to setup/import screens.
- Local database write access through the application.
- Local read access to the selected employee file.

## Duplicate Rules

Employee import blocks duplicates by:

- `EmployeeNumber` when an employee number is mapped.
- `FirstName` + `LastName` + `SSNLast4` when SSN last four is mapped.

The duplicate checks compare against both the selected local company and the rows in the selected file.

## Operational Steps

1. Prepare a local employee file with headers.
2. Open `/import/employees`.
3. Select the target company.
4. Select the local file.
5. Map employee fields.
6. Validate rows.
7. Correct blocking errors in the source file or mapping.
8. Confirm import.
9. Review imported employees on `/setup/employees`.

## Safety Notes

- Files are parsed locally.
- No cloud upload is added.
- No telemetry is added.
- No ACH submission is added.
- No tax filing is added.
- Full SSNs, full account numbers, and routing numbers are rejected when mapped.
