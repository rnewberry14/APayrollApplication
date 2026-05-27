# Employee Payroll Profile - Troubleshooting

## What This Feature Does

Employee payroll profile models store local payroll setup data and validation rules for future employee setup screens.

## Who Can Use It

Support users and administrators troubleshooting local employee setup records.

## Required Permissions

- Read employee setup records.
- Review direct deposit token setup records.

## Common Errors

| Symptom | Likely Cause | Troubleshooting Step |
| --- | --- | --- |
| Employee does not validate | Missing required demographic or pay fields | Review required fields and pay type |
| SSN field fails validation | More than four digits were entered | Keep only SSN last four |
| Bank setup fails validation | Plain numeric routing or account number was entered | Replace with token placeholder values |
| Direct deposit split fails validation | Account count, priority, remainder, amount, or percent is invalid | Review active direct deposit records |

## Cautions

- Do not request or display full SSNs or full bank account numbers.
- Do not submit real ACH from setup records.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
