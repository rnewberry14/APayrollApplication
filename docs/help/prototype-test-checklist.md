# End-to-End Prototype Test Checklist - User Guide

## Overview

The End-to-End Prototype Test Checklist is a local testing page at `/prototype-test-checklist`. It gives testers a manual checklist for walking through the ClearPath Payroll prototype from demo data through payroll preview, approval, fake/sandbox direct deposit, pay stubs, reports, CSV exports, and sensitive-data checks.

This page is for local prototype testing only. It does not transmit data externally, submit ACH, submit tax filings, or call production APIs.

## Who Can Use It

Payroll testers, developers, and administrators working in Development or Local Prototype Mode can use this checklist.

## Required Permissions

- Access to the local ClearPath Payroll prototype.
- Permission to seed demo data in the local database.
- Permission to open payroll setup, payroll run, report, and pay stub pages.

## Step-by-Step Instructions

1. Open `/prototype-test-checklist`.
2. Confirm Local Prototype Mode is enabled.
3. Click **Seed Data** and seed local demo records.
4. Review Company Setup or the seeded company data.
5. Review Employee Setup or the seeded employee data.
6. Open Pay Schedule Setup and confirm pay schedule dates before processing payroll.
7. Open Create Payroll and create a draft payroll run.
8. On Payroll Preview, click **Recalculate Payroll**.
9. Review payroll totals and validation messages.
10. Click **Approve Payroll** and confirm the approval modal.
11. Submit fake/sandbox direct deposit only after the run is Approved.
12. Open a pay stub for a payroll run employee.
13. Open Payroll Register.
14. Open Tax Liability Report.
15. Export available CSV reports.
16. Confirm no real ACH was submitted.
17. Confirm no tax filing was submitted.
18. Confirm no full SSN or full bank account number is displayed.
19. Mark each checklist item complete as you finish it.

## Common Errors

| Error | Cause | Resolution |
| --- | --- | --- |
| Checklist controls are disabled | The app is not running in Development or Local Prototype Mode | Start the app in Development or enable Local Prototype Mode in a non-production environment |
| No demo data appears | Demo data was not seeded or the local database was reset | Open Seed Demo Data and run the seed operation again |
| Payroll cannot be approved | The run has not been calculated or validation errors exist | Recalculate payroll and resolve validation errors before approval |
| Fake direct deposit cannot be submitted | Payroll is not Approved or a fake batch already exists | Approve the payroll run first and avoid duplicate submissions |

## Troubleshooting

- If the checklist is unavailable, confirm the environment is Development or local prototype testing.
- If Company Setup or Employee Setup pages are not present, review seeded company and employee data through available local prototype screens or database inspection tools.
- If a pay stub route requires IDs, use the payroll run ID and employee ID from the created prototype payroll run.
- If reports are empty, confirm payroll was calculated and approved for the selected period.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not enter real SSNs, full bank account numbers, routing numbers, production API keys, or real payment instructions.
- Do not submit real ACH from this prototype workflow.
- Do not submit tax filings from this prototype workflow.
- Confirm reports and pay stubs show masked sensitive data only.
