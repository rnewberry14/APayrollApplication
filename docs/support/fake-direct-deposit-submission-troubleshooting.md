# Fake Direct Deposit Submission - Troubleshooting

## What the Feature Does

Fake Direct Deposit Submission creates a sandbox direct deposit batch for an approved payroll run and displays the batch status plus fake external reference.

## Who Can Use It

Payroll administrators, payroll managers, and support users with sandbox direct deposit access.

## Required Permissions

- View payroll run.
- View direct deposit batch status.
- Submit sandbox direct deposit.

## Common Errors

| Symptom | Likely Cause | Troubleshooting Step |
| --- | --- | --- |
| Button is not visible | Payroll run is not Approved or already submitted | Approve payroll or review existing batch |
| Missing verified bank account | Employee bank account is missing, inactive, or pending | Add or verify a sandbox bank account |
| Missing funding account | Company has no active funding account | Add active sandbox company funding |
| Production ACH blocked | Non-fake ACH service is configured | Switch to `FakeAchPaymentService` for local prototype |

## Troubleshooting Steps

1. Confirm the payroll run status is Approved.
2. Confirm no direct deposit batch already exists.
3. Confirm employee bank accounts are active and verified.
4. Confirm company funding account is active.
5. Submit again and record the fake reference if successful.

## Payroll, Tax, ACH, and Security Cautions

- Do not request real bank data.
- Do not show full account numbers.
- Do not show routing numbers.
- Do not use production ACH.
- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
