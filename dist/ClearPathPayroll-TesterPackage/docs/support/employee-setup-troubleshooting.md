# Employee Setup - Troubleshooting

## What This Feature Does

Employee Setup saves local employee payroll profile records and validates sensitive setup rules.

## Who Can Use It

Support users and administrators troubleshooting local employee setup.

## Required Permissions

- View employee setup records.
- Edit employee setup records.

## Common Errors

| Symptom | Likely Cause | Troubleshooting Step |
| --- | --- | --- |
| Save fails on SSN | Value is not exactly four digits | Enter SSN last four only |
| Save fails on direct deposit | Raw bank data or invalid split setup | Use token placeholder values and review deposit type |
| Employee list is empty | No company selected or no employees exist | Select a company or add an employee |
| Pay validation fails | Missing hourly rate or annual salary | Enter the rate matching the pay type |

## Cautions

- Do not request or display full SSNs, full routing numbers, or full bank account numbers.
- Employee Setup does not transmit data externally.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
