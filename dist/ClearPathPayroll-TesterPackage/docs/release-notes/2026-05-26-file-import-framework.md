# 2026-05-26 File Import Framework

Added a local file import framework for CSV, tab-delimited text, and `.xlsx` files.

## Added

- Import batch, row, error, and mapping models.
- Local parser interface and CSV/tab-delimited/Excel parser implementations.
- Import service for parse, map, validate, preview, and confirm workflow.
- `/setup/imports` page with local-only warnings, mapping, preview, validation, and confirmation.
- Navigation link under Setup.
- Unit tests for CSV parsing, tab-delimited parsing, and import service validation/confirmation.
- Documentation for help, admin, support, technical, and release notes.

## Safety Notes

- No cloud upload was added.
- No telemetry was added.
- No external tax API call was added.
- No ACH or tax filing submission was added.
- The workflow does not store original files permanently.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
