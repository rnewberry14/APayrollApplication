# 2026-05-26 - Employee Payroll Profile Models

## Summary

Added local employee payroll profile model fields for future complete employee setup screens.

## Changes

- Expanded employee demographic, employment, pay, federal W-4, state tax, notes, and timestamp fields.
- Extended direct deposit setup to support tokenized setup for up to five active accounts.
- Added user-defined employee payroll fields.
- Added EF Core configuration and migration.
- Added validation tests.

## Cautions

- Full SSNs and full bank account numbers are not stored in plain text.
- Direct deposit setup records do not submit real ACH.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
