# Fake Direct Deposit Submission - Admin Guide

## What the Feature Does

Fake Direct Deposit Submission lets an approved payroll run create a sandbox direct deposit batch using `FakeAchPaymentService` only. It records batch status and a fake external batch reference.

## Who Can Use It

Payroll administrators and authorized payroll managers in local prototype workflows.

## Required Permissions

- Payroll run read access.
- Direct deposit batch create access.
- Company funding account read access.
- Employee bank account verification read access.

## Admin Steps

1. Confirm the application is configured with `FakeAchPaymentService`.
2. Confirm the payroll run is Approved.
3. Confirm the company has an active funding account.
4. Confirm each direct deposit employee has an active verified bank account.
5. Submit fake/sandbox direct deposit from Payroll Preview.
6. Confirm batch status and fake external reference are shown.

## Common Errors

- Production ACH service configured: submission is blocked.
- Missing funding account: add an active sandbox funding account.
- Missing employee bank account: add a verified sandbox employee bank account.
- Duplicate submission: use the existing batch instead of submitting again.

## Troubleshooting

1. Check dependency injection uses `FakeAchPaymentService`.
2. Review `DirectDepositBatches` for an existing submitted or settled batch.
3. Verify bank account records store tokens and last four only.
4. Review audit logs for fake submission events.

## Payroll, Tax, ACH, and Security Cautions

- Do not transmit real bank data.
- Do not show full bank account numbers.
- Do not show routing numbers.
- Do not use production ACH.
- This feature does not submit tax filings.
