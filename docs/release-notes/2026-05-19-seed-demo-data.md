# Release Notes: Seed Demo Data

Date: 2026-05-19

Summary

- Added a Development-only Seed Demo Data feature to populate LocalDB with sample payroll data for demos and testing.

Details

- New `SeedDataService` that writes a demo company, three employees (hourly, salary, direct deposit), pay schedule, draft payroll run, earnings, deductions, tax lines, net pay lines, a demo employee bank account (tokenized), and a demo company funding account.
- New Blazor page `Seed Demo Data` (visible only in Development or Local Prototype Mode) to trigger seeding.
- Service is protected by `PrototypeModeHelper` and not allowed in Production.

Notes

- All seeded values are clearly fake (Last4 = 0000, tokens contain "demo-").
- No real SSNs or bank routing/account numbers are used.
