# Payroll Run Setup - Troubleshooting

## What the Feature Does

Payroll Run Setup creates draft payroll runs for local prototype review. It selects a company, pay schedule, and eligible employees, then records regular earnings and manual gross pay adjustments.

## Who Can Use It

Payroll administrators, payroll managers, and support staff with appropriate setup access.

## Required Permissions

- View company setup data.
- View pay schedules.
- View employees.
- Create draft payroll runs.

## Common Errors

| Symptom | Likely Cause | Troubleshooting Step |
| --- | --- | --- |
| Page says prototype only | Environment is not allowed | Use Development or enable Local Prototype Mode |
| No pay schedules | Company has no active schedule | Create or activate a pay schedule |
| No employees | No active employees for company | Create or activate employees |
| Create button disabled | No employees selected | Select at least one employee |
| Preview shows zero taxes | Taxes are not calculated on this page | Continue to the tax calculation workflow when ready |

## Troubleshooting Steps

1. Confirm the user is on `/payroll/create`.
2. Confirm the selected company is active.
3. Confirm the selected pay schedule is active and belongs to the company.
4. Confirm selected employees are active and belong to the company.
5. Confirm hours and manual adjustments are not negative.
6. Check logs for database save failures.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not submit ACH from this page.
- Do not submit tax filings from this page.
- Do not call production APIs from this workflow.
- Do not ask users to send SSNs, bank account numbers, routing numbers, tax IDs, or API keys.
