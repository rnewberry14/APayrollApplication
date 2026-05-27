# 2026-05-26 QuickBooks Online Import

Added local-file QuickBooks Online import templates and workflow.

## Added

- `/import/quickbooks-online`
- QBO export templates and aliases
- Local parse, map, preview, validate, and confirm workflow
- Documentation and tests

## Safety Notes

- No OAuth was added.
- No direct Intuit API was added.
- No Intuit credentials are stored.
- No files are uploaded to cloud storage.
- No ACH or tax filing submission is performed.
