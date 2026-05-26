# Limited Liability Mode - Technical Notes

## Overview

Limited Liability Mode adds local-only startup configuration and validation for ClearPath Payroll.

## Implementation

- Options: `LimitedLiabilityModeOptions`
- Helper: `LimitedLiabilityModeHelper`
- Startup wiring: `Program.cs`
- Home banner: `Components/Pages/Home.razor`
- Reset UI: `Components/Pages/SeedDemoData.razor`

## Database Behavior

When Limited Liability Mode is active, `PayrollDbContext` uses the configured local database provider. Development defaults to SQLite:

```text
Data Source=src/ClearPathPayroll/App_Data/{LimitedLiabilityMode:LocalDatabaseName}.db
```

Development defaults to `ClearPathPayroll.LocalOnly.Dev`, which creates `src/ClearPathPayroll/App_Data/ClearPathPayroll.LocalOnly.Dev.db`. SQLite startup uses `EnsureCreated`; SQL Server LocalDB remains supported and uses EF migrations.

Local prototype Data Protection keys are stored under `src/ClearPathPayroll/App_Data/DataProtectionKeys` to avoid stale user-profile keys during local testing.

## External Services

- `ITaxCalculationService` uses `FakeTaxCalculationService` when `AllowExternalTaxApiLookup` is false.
- `IAchPaymentService` remains wired to `FakeAchPaymentService`.
- No telemetry provider is added.
- No real tax filing service is added.

## Validation

`LimitedLiabilityModeHelper.ValidateStartupSafety` blocks local demo startup when telemetry, real ACH, or real tax filing are enabled.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not add production provider transmissions to local-only mode.
- Do not log SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets.
