# 2026-05-21 - Payroll Run Setup

## Summary

Added a local prototype page for creating draft payroll runs.

## Changes

- Added `/payroll/create`.
- Added company and pay schedule selection.
- Displayed pay period start, pay period end, and pay date from the selected schedule.
- Added eligible employee selection.
- Added regular hours entry for hourly employees.
- Added optional manual gross pay adjustment.
- Created draft payroll runs and payroll run employee records.
- Added basic earning lines and a payroll run creation audit log.
- Navigates to Payroll Preview after creation.
- Added user, admin, support, and technical documentation.

## Cautions

- This is a local prototype workflow.
- Taxes are not calculated on this page.
- ACH is not submitted from this page.
- Tax filings are not submitted from this page.
- Production APIs are not called from this page.
