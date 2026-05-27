# 2026-05-26 Tax Deposits Import

Added Tax Deposits Only import workflow.

## Added

- `/import/tax-deposits`
- `TaxDepositRecord` model and database migration
- CSV, tab-delimited, and `.xlsx` parsing through the generic import framework
- Mapping, preview, validation, and confirmation workflow
- Tax Liability Report user-entered deposit records section
- Documentation and tests

## Safety Notes

- No tax payment submission is performed.
- No tax filing submission is performed.
- No IRS or state system is called.
- Imported deposits are displayed as `User-entered deposit record` entries.
- ClearPath Payroll does not claim imported deposit records are verified.
