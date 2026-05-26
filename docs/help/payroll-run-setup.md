# Payroll Run Setup - User Guide

## Overview

Payroll Run Setup creates a draft payroll run from an active company, pay schedule, and selected active employees. It captures regular hours for hourly employees and optional manual gross pay adjustments.

This is a local prototype workflow. It does not calculate taxes, submit ACH, submit tax filings, or call production APIs.

## Who Can Use It

Payroll managers and payroll administrators who are allowed to create draft payroll runs can use this page.

## Required Permissions

- View active companies.
- View active pay schedules.
- View active employees.
- Create draft payroll runs.

## Step-by-Step Instructions

1. Open **Create Payroll** from the navigation menu.
2. Select a company.
3. Select an active pay schedule.
4. Review the pay period start, pay period end, and pay date.
5. Select eligible employees.
6. Enter regular hours for hourly employees.
7. Optionally enter a positive manual gross pay adjustment.
8. Review the draft gross pay total.
9. Click **Create Payroll Run**.
10. The app opens the Payroll Preview page for the new draft run.

## Common Errors

| Error | Cause | Resolution |
| --- | --- | --- |
| Select a company | No company was selected | Choose an active company |
| Select a pay schedule | No active pay schedule was selected | Choose or create an active pay schedule |
| Select at least one employee | No employees were checked | Select one or more eligible employees |
| Hours cannot be negative | A negative hours value was entered | Enter zero or a positive number |

## Troubleshooting

- If no companies appear, create or activate a company.
- If no schedules appear, create an active pay schedule for the company.
- If no employees appear, create or activate employees for the company.
- If the preview page shows zero taxes, continue to the calculation workflow when ready.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not submit ACH from this page.
- Do not submit tax filings from this page.
- Do not call or configure production APIs for this prototype workflow.
- Do not enter SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets into manual fields.
