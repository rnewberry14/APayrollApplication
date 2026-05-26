# Release Notes: Demo Data Seeder

## Summary

Added a development/local-only demo data seeder for local payroll workflow review.

## Changes

- Added `DemoDataSeeder`.
- Added `/demo/seed-data`.
- Added **Create Demo Data** and **Clear Demo Data** buttons.
- **Create Demo Data** now repairs partial demo datasets instead of stopping when only `Demo Company LLC` exists.
- Updated prototype checklist and navigation to use `/demo/seed-data`.
- Enabled Interactive Server rendering for app routes so demo page buttons execute their Blazor click handlers.
- Seeded Demo Company LLC, three fake employees, one pay schedule, one draft payroll run, fake verified direct deposit tokens, and a fake company funding account token.
- Added tests and documentation.

## Caution

Demo data is fake and must not be used for real payroll.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
