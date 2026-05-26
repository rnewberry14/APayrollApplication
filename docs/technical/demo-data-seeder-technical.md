# Demo Data Seeder - Technical Notes

## Overview

`DemoDataSeeder` creates and clears fake local demo records for ClearPath Payroll. It is separate from the older `SeedDataService` so the new demo dataset can use the exact requested fake company and employee records.

## Implementation

- Service: `src/ClearPathPayroll/Services/DemoDataSeeder.cs`
- Page: `src/ClearPathPayroll/Components/Pages/DemoSeedData.razor`
- Route: `/demo/seed-data`
- The seeder is repairable/idempotent: if `Demo Company LLC` already exists, `CreateDemoDataAsync` verifies and recreates missing demo employees, pay schedule, draft payroll run, fake funding token, verified fake employee bank tokens, and payroll run employee earning lines.
- DI registration: `Program.cs`
- Tests: `tests/ClearPathPayroll.Tests/DemoDataSeederTests.cs`

## Guardrails

The service allows seeding only when:

- `IHostEnvironment.IsDevelopment()` is true, or
- `LimitedLiabilityMode:Enabled` is true.

The service does not call tax APIs, ACH APIs, telemetry, or external submission services.

## Seeded Records

- `Company`: Demo Company LLC, FEIN 00-0000000, State OK.
- `Employee`: one hourly employee, one salary employee, and one tipped placeholder employee.
- `EmployeeBankAccount`: verified fake direct deposit token records.
- `CompanyFundingAccount`: fake company funding account token.
- `PaySchedule`: one biweekly schedule.
- `PayrollRun`: one draft payroll run.
- `PayrollRunEmployee` and `EarningLine`: starter local review lines, including a tips placeholder line.

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not store full SSNs, real bank account numbers, or routing numbers in demo seed data.
