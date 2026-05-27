# 2026-05-26 Employees Only Import

Added a dedicated Employees Only Import workflow.

## Added

- `/import/employees` page.
- Company selection.
- CSV, tab-delimited, and `.xlsx` parsing through the generic import framework.
- Employee column mapping and preview.
- Employee-specific validation.
- Duplicate prevention by employee number or name plus SSN last four.
- Confirmed local creation of employee records.
- Documentation and tests.

## Safety Notes

- No cloud upload was added.
- No telemetry was added.
- No ACH submission was added.
- No tax filing was added.
- Full SSNs, full bank account numbers, and routing numbers are not accepted when mapped.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
