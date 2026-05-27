# Release Notes: Limited Liability Mode

## Summary

Added Limited Liability Mode for local-only ClearPath Payroll prototype operation.

## Changes

- Added `LimitedLiabilityMode` configuration.
- Added startup validation for telemetry, real ACH submission, and real tax filing in local demo mode.
- Added local-only database selection.
- Development local-only storage now defaults to SQLite at `src/ClearPathPayroll/App_Data/ClearPathPayroll.LocalOnly.Dev.db` so prototype testing does not require a working SQL Server LocalDB instance.
- Local prototype Data Protection keys are stored under `src/ClearPathPayroll/App_Data/DataProtectionKeys`.
- Switched local-only tax calculation wiring to the fake/local tax calculation service unless external lookup is explicitly allowed.
- Added the required home page local-storage banner.
- Added Clear Demo Data action to the Seed Demo Data page.
- Added validation tests and documentation.

## Caution

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
