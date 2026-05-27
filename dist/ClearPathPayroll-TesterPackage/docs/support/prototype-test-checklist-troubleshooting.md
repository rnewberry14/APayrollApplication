# End-to-End Prototype Test Checklist - Troubleshooting

## Overview

This guide helps support staff troubleshoot the local End-to-End Prototype Test Checklist at `/prototype-test-checklist`.

## Who Can Use It

Support staff, developers, and administrators supporting local prototype testing can use this guide.

## Required Permissions

- Access to the local prototype application.
- Access to application logs when troubleshooting failures.
- Permission to inspect non-sensitive local demo data.

## Step-by-Step Use Instructions

1. Ask the tester to open `/prototype-test-checklist`.
2. Confirm whether the page says Local Prototype Mode is available.
3. Have the tester seed demo data.
4. Have the tester continue through each checklist item in order.
5. Record which checklist step first fails.
6. Use the troubleshooting table below for the failed step.

## Common Errors

| Problem | Likely Cause | Resolution |
| --- | --- | --- |
| Checklist controls are disabled | Environment is not Development or Local Prototype Mode | Restart with the correct local environment or enable prototype mode outside production |
| Seed demo data fails | Database is unavailable or migrations are missing | Verify local database connection and apply migrations |
| Create Payroll has no company, schedule, or employee | Demo data was not seeded or records are inactive | Rerun Seed Demo Data and confirm active records |
| Approval button is disabled | Payroll is still Draft or validation errors exist | Click Recalculate Payroll and resolve validation errors |
| Fake direct deposit fails | Missing or unverified demo bank token, unapproved payroll, or duplicate batch | Use seeded demo bank data, approve payroll first, and use a run without an existing fake batch |
| Pay stub route fails | Wrong payroll run ID or employee ID | Use IDs from the created payroll run employee records |

## Troubleshooting Steps

- Confirm the tester did not use real personal, bank, or employer data.
- Confirm no production API configuration is being used.
- Review Payroll Preview errors before approval.
- Check whether a direct deposit batch already exists for the payroll run.
- Verify exports do not include full SSNs, full bank account numbers, or routing numbers.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not transmit real bank data.
- Do not show full bank account numbers or routing numbers.
- Do not submit real ACH.
- Do not submit tax filings.
- Do not treat checklist completion as a compliance conclusion.
