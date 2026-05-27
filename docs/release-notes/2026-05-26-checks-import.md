# 2026-05-26 Checks Import

Added Checks Only and Employees And Checks import workflows for after-the-fact payroll entry.

## Added

- `/import/checks`
- `/import/employees-and-checks`
- CSV, tab-delimited, and `.xlsx` parsing through the generic import framework
- Mapping, preview, validation, and confirmation workflow
- Net pay total mismatch validation
- Draft after-the-fact payroll run creation
- Earning, deduction, tax, and net pay lines
- Audit log entry for imported payroll runs
- Documentation and tests

## Safety Notes

- Imported checks are not auto-approved.
- No ACH submission is performed.
- No tax filing is performed.
- No cloud upload or telemetry is added.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
