# Pay Schedule Setup - Troubleshooting

## What the Feature Does

Pay Schedule Setup creates pay schedules for active companies and shows a calculated preview of the following pay period.

## Who Can Use It

Payroll administrators, payroll managers, and support staff with appropriate access may use or support this page.

## Required Permissions

- View company setup data.
- View pay schedules.
- Create pay schedules.

## Common Errors

| Symptom | Likely Cause | Troubleshooting Step |
| --- | --- | --- |
| No companies are listed | No active company exists | Ask an administrator to create or activate the company |
| Save button does not create a schedule | Validation failed | Review the validation messages above the form |
| Preview is not shown | Period dates are invalid | Make the period end date on or after the start date |
| Schedule saved as inactive | Is active was unchecked | Create or update an active schedule before using it for payroll |
| Dates are wrong after save | Entered values were incorrect | Confirm dates and create a corrected schedule if needed |

## Step-by-Step Support Checks

1. Confirm the user is on `/setup/pay-schedules`.
2. Confirm an active company is selected.
3. Check validation messages.
4. Verify the frequency and dates entered by the user.
5. Confirm the saved schedule appears in the table.
6. If saving fails, review application logs for the exception message.

Confirm pay schedule dates before processing payroll.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not ask users to send SSNs, bank account numbers, routing numbers, employer tax IDs, or API keys.
- Pay schedule dates may affect payroll processing, tax API requests, and direct deposit timing.
- Support notes should avoid sensitive payroll and banking details.
