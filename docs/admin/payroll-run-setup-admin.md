# Payroll Run Setup - Admin Guide

## What the Feature Does

Payroll Run Setup creates a draft `PayrollRun` and selected `PayrollRunEmployee` records. It uses the selected pay schedule dates and captures regular earnings plus optional positive manual gross pay adjustments.

This page is intended for local prototype use only.

## Who Can Use It

Payroll administrators and payroll managers with payroll run creation permissions.

## Required Permissions

- Company read access.
- Pay schedule read access.
- Employee read access.
- Payroll run create access.

## Admin Steps

1. Confirm Local Prototype Mode or Development environment is active.
2. Confirm the company is active.
3. Confirm the company has an active pay schedule.
4. Confirm eligible employees are active.
5. Open `/payroll/create`.
6. Select the company and pay schedule.
7. Select employees and enter hours or adjustments.
8. Create the draft payroll run.
9. Review the run on Payroll Preview.

## Common Errors

- Page unavailable: environment is not Development or Local Prototype Mode.
- Schedule missing: no active pay schedule exists for the company.
- Employee missing: employee is inactive, terminated, or attached to another company.
- Gross pay unexpected: verify hourly rates, salary values, frequency, hours, and manual adjustment.

## Troubleshooting

1. Check `PrototypeMode` settings.
2. Verify company, employee, and schedule records in the database.
3. Review application logs for save errors.
4. Confirm no tax or ACH service was invoked from this workflow.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not submit ACH.
- Do not submit tax filings.
- Do not call production APIs.
- Do not store SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets in notes or adjustment descriptions.
