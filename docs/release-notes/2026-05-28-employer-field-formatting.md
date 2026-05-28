# 2026-05-28 Employer Field Formatting

## Summary

Updated employer setup and payroll settings field behavior for the local demo application.

## Changes

- FEIN values are no longer replaced with `00-0000000` or shown with fake masked first digits in employer setup.
- Added SUIN and SEIN fields to employer payroll settings.
- Added phone number formatting to `(123) 456-7890`.
- Added percent rate parsing and 4-decimal formatting.
- Replaced SUTA state text entry with a state dropdown.
- Renamed filing frequency entry to Filing Frequency / Depositor Type and added dropdown options.

## Safety Notes

ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice. User-entered rate. Verify with official agency records. This change does not add ACH submission, tax filing, telemetry, or external service calls.
