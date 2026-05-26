# Limited Liability Mode - User Guide

## Overview

Limited Liability Mode keeps ClearPath Payroll in a local-only safety posture. Payroll data is stored in a database on the user's computer or on a private network server selected by the user. The mode disables telemetry, real ACH submission, and real tax filing for local demo use.

Reports and analytics are information-only displays based on user-entered and locally stored data. ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Where Data Is Stored

In the default local setup, data is stored in a SQLite database file on this computer:

- Provider: `SQLite`
- Development database file: `src/ClearPathPayroll/App_Data/ClearPathPayroll.LocalOnly.Dev.db`
- Default database file: `src/ClearPathPayroll/App_Data/ClearPathPayroll.LocalOnly.db`

SQL Server LocalDB remains supported when the machine has a working LocalDB instance. The app does not configure Azure SQL as the default database. Users may configure their own storage accounts or private network servers outside this local demo workflow.

## Step-by-Step Use

1. Open ClearPath Payroll locally.
2. Confirm the home page banner says: "Data is stored on this computer. No real ACH or tax filing is enabled."
3. Open Seed Demo Data.
4. Click Seed Demo Data to create local demo records.
5. Use the prototype checklist and payroll pages with local demo data only.
6. Click Clear Demo Data on the Seed Demo Data page to remove the seeded demo company and related demo records.

## Required Permissions

- Permission to run the app locally.
- Permission to create and use files under `src/ClearPathPayroll/App_Data`.
- Permission to seed and clear local demo data.

## Common Errors

| Error | Cause | Resolution |
| --- | --- | --- |
| Startup blocked | Telemetry, real ACH, or real tax filing was enabled in local demo mode | Set those options to `false` |
| Seed data fails | Database schema is missing or the app cannot write to `App_Data` | Restart the app and confirm the user has write access to `src/ClearPathPayroll/App_Data` |

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not enter real SSNs, full bank account numbers, routing numbers, employer tax IDs, or production API keys in local demo data.
- Limited Liability Mode does not submit real ACH or tax filings.
