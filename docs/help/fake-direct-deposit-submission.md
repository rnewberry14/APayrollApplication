# Fake Direct Deposit Submission - User Guide

## Overview

Fake Direct Deposit Submission creates a sandbox direct deposit batch for an approved payroll run. It is for local prototype testing only and does not move real money.

## Who Can Use It

Payroll administrators and payroll managers who are allowed to submit sandbox direct deposit batches.

## Required Permissions

- View payroll runs.
- View approved payroll totals.
- Submit fake/sandbox direct deposit batches.

## Step-by-Step Instructions

1. Create a payroll run.
2. Recalculate payroll.
3. Approve payroll.
4. On Payroll Preview, find **Fake/Sandbox Direct Deposit**.
5. Click **Submit Direct Deposit**.
6. Review the confirmation modal.
7. Click **Confirm Fake Submission**.
8. Review the batch status and fake batch reference.

## Common Errors

| Error | Cause | Resolution |
| --- | --- | --- |
| Only Approved payroll runs can be submitted | Payroll is not approved | Approve payroll first |
| Already has a direct deposit batch | Duplicate submission was attempted | Use the existing batch status and reference |
| Missing active verified bank account | Employee bank setup is missing or pending | Add a verified sandbox bank account |
| No active company funding account | Company funding setup is missing | Add an active sandbox funding account |

## Troubleshooting

- Confirm the payroll run status is Approved.
- Confirm each direct deposit employee has an active verified bank account.
- Confirm the company has an active funding account.
- Review the fake batch status and fake reference after submission.

## Payroll, Tax, ACH, and Security Cautions

- This is fake/sandbox direct deposit only.
- Do not transmit real bank data.
- Do not enter or show full bank account numbers.
- Do not enter or show routing numbers.
- Do not use production ACH.
