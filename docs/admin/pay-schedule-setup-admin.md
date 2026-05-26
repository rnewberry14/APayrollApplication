# Pay Schedule Setup - Admin Guide

## What the Feature Does

The Pay Schedule Setup page creates company-specific pay schedules used by payroll workflows. It supports Weekly, Biweekly, Semimonthly, and Monthly frequencies and previews the following pay period based on the entered dates.

## Who Can Use It

Payroll administrators and company setup managers should use this feature. Limit access to users who are trusted to configure payroll calendars.

## Required Permissions

- Company setup access.
- Pay schedule create access.
- Pay schedule view access.

## Admin Use Steps

1. Confirm the company exists and is active.
2. Open `/setup/pay-schedules`.
3. Select the company.
4. Enter a descriptive schedule name.
5. Select the frequency.
6. Enter the next pay date and next period dates.
7. Review the preview for the following pay period.
8. Save the schedule.
9. Confirm the schedule appears in the company schedule list.

Confirm pay schedule dates before processing payroll.

## Common Errors

- No companies are available: create or reactivate a company first.
- Schedule does not appear in payroll workflows: verify the schedule is active.
- Preview dates look unexpected: review the frequency and the entered period dates.

## Troubleshooting

1. Verify the user has company setup permissions.
2. Check that the selected company is active.
3. Confirm the pay period end date is not before the start date.
4. Confirm the pay date is not before the period end date.
5. Review application logs for save failures without logging sensitive payroll data.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Incorrect dates can affect payroll calculations, tax API requests, pay stubs, payroll registers, and ACH timing.
- Do not store SSNs, full bank account numbers, routing numbers, employer tax IDs, or secrets in schedule names.
- Production ACH submission should remain behind approved payroll and sandbox controls.
