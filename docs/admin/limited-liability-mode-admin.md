# Limited Liability Mode - Admin Guide

## Overview

Limited Liability Mode is configured with the `LimitedLiabilityMode` section. It is intended to keep local and demo use on user-controlled storage with no telemetry, no real ACH submission, and no real tax filing.

## Configuration

```json
"LimitedLiabilityMode": {
  "Enabled": true,
  "AllowExternalTaxApiLookup": false,
  "AllowRealAchSubmission": false,
  "AllowRealTaxFiling": false,
  "AllowTelemetry": false,
  "LocalDatabaseProvider": "SqlServerLocalDb",
  "LocalDatabaseName": "ClearPathPayroll.LocalOnly"
}
```

## Where Data Is Stored

By default, local data is stored in SQL Server LocalDB on the user's computer. Development uses `ClearPathPayroll.LocalOnly.Dev`. The app does not use Azure SQL as the default database.

Private network database servers or user-provided storage may be configured by the owner outside the local demo defaults. Do not configure storage accounts, hosted databases, telemetry, or provider transmissions that are not provided and controlled by the user.

## Required Permissions

- Local machine permission to create a SQL Server LocalDB database.
- Admin access to app configuration.
- Permission to seed and clear local demo data.

## Startup Blocks

The app will block startup in local demo mode if any of these are true:

- `LimitedLiabilityMode:AllowTelemetry`
- `LimitedLiabilityMode:AllowRealAchSubmission`
- `LimitedLiabilityMode:AllowRealTaxFiling`

## Reset Local Demo Data

1. Open `/seed-demo-data`.
2. Click Clear Demo Data.
3. Click Seed Demo Data again if you want a fresh local demo dataset.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not enable telemetry, real ACH submission, or real tax filing in local demo mode.
- Do not store secrets, SSNs, full bank account numbers, or routing numbers in demo records.
