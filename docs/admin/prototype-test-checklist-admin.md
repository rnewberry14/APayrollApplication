# End-to-End Prototype Test Checklist - Admin Guide

## Overview

The End-to-End Prototype Test Checklist helps administrators and testers run a repeatable local prototype test path. It is available at `/prototype-test-checklist` and stores checkbox status only in the current page session.

## Who Can Use It

Administrators, payroll testers, and developers who have access to the local prototype environment can use this page.

## Required Permissions

- Access to Development or Local Prototype Mode.
- Local database access through the application.
- Permission to seed demo data.
- Permission to create, calculate, preview, approve, and report on prototype payroll runs.

## Step-by-Step Use

1. Confirm the environment is Development or Local Prototype Mode.
2. Open `/prototype-test-checklist`.
3. Seed demo data.
4. Walk through company, employee, pay schedule, payroll run, payroll preview, approval, fake direct deposit, pay stub, payroll register, tax liability report, and CSV export checks.
5. Mark each checklist item complete manually.
6. Reset the checklist if you need to repeat the test path.

## Common Errors

| Error | Cause | Admin Action |
| --- | --- | --- |
| Prototype mode unavailable | App is running as Production or prototype mode is disabled | Use Development or a non-production environment with prototype mode enabled |
| Seed data button is unavailable | Seed Demo Data is also gated by prototype mode | Confirm environment configuration |
| Fake direct deposit blocked | Payroll run is not Approved or a fake batch already exists | Use a calculated and approved run without an existing fake batch |
| Pay stub cannot open | Payroll run ID or employee ID is missing | Use IDs from the created prototype payroll run |

## Troubleshooting Steps

- Check application configuration for `PrototypeMode`.
- Confirm the local database is reachable.
- Rerun Seed Demo Data after local database resets.
- Review the Payroll Preview validation messages before approval.
- Confirm any direct deposit reference shown is fake/sandbox only.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not enable this workflow in production.
- Do not use production ACH or tax filing integrations while testing this checklist.
- Do not enter or display full SSNs, routing numbers, full bank account numbers, employer tax IDs, or API secrets.
