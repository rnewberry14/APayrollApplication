# Demo Data Seeder - User Guide

## Overview

The Demo Data Seeder creates or repairs fake local payroll records for manual review, testing, and documentation. It is available at `/demo/seed-data` in Development or Local-Only Mode.

Demo data is fake and must not be used for real payroll.

## What It Creates

- One fake company: Demo Company LLC.
- Three fake employees: hourly, salary, and tipped employee placeholder.
- Fake SSN last four values only.
- One pay schedule.
- One draft payroll run.
- Verified fake employee direct deposit token records.
- One fake company funding account token.

## How To Create Demo Data

1. Run ClearPath Payroll in Development or Local-Only Mode.
2. Open `/demo/seed-data`.
3. Click **Create Demo Data**.
4. If a previous seed created only some records, click **Create Demo Data** again to repair missing employees, pay schedule, draft payroll run, fake funding token, verified fake bank tokens, and payroll run employee lines.
4. Review the summary showing company, employee count, pay schedule ID, and draft payroll run ID.

## How To Clear Demo Data

1. Open `/demo/seed-data`.
2. Click **Clear Demo Data**.
3. The page removes the seeded demo company and related demo records.

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not use real SSNs, real bank account numbers, real routing numbers, production API keys, or real payroll instructions.
- This page does not call tax APIs, call ACH APIs, or submit anything externally.
