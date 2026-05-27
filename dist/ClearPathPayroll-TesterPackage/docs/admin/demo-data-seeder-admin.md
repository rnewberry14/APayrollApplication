# Demo Data Seeder - Admin Guide

## Overview

The Demo Data Seeder is a Development and Local-Only Mode tool for creating fake payroll records. It is intended for local manual review, testing, and documentation.

## Access

- Route: `/demo/seed-data`
- Service: `DemoDataSeeder`
- Allowed environments: Development or Local-Only Mode

## Admin Steps

1. Confirm the app is running locally with `LimitedLiabilityMode:Enabled` set to `true`, or in Development.
2. Open `/demo/seed-data`.
3. Click **Create Demo Data**.
4. Run the payroll workflow using the fake demo records.
5. Click **Clear Demo Data** when testing is complete.

## Data Safety

- The seeder stores fake SSN last four values only.
- It does not store full SSNs.
- It stores fake routing and account tokens only.
- It does not call tax APIs or ACH APIs.
- It does not submit payments, filings, or external records.

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Demo data is fake and must not be used for real payroll.
