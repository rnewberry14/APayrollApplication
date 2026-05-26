# Payroll Preview Calculation and Approval - User Guide

## Overview

Payroll Preview lets authorized users calculate a draft payroll run, review totals, approve a calculated payroll run, and create a fake/sandbox direct deposit batch after approval when Local-Only Mode is enabled.

The page does not submit real direct deposit or submit tax filings.

## Who Can Use It

Payroll managers and payroll administrators who can review, calculate, and approve payroll runs.

## Required Permissions

- View payroll runs.
- Calculate payroll.
- Approve payroll.
- Submit fake/sandbox direct deposit in local prototype mode.

## Step-by-Step Instructions

1. Open a payroll run on Payroll Preview.
2. Review the company, pay period, pay date, selected employees, gross pay, deductions, taxes, and net pay.
3. Click **Recalculate Payroll**.
4. Wait for the success message and confirm the status is Calculated.
5. Review any validation errors or warnings.
6. Click **Approve Payroll**.
7. Review the confirmation modal.
8. Click **Confirm Approval**.
9. Confirm the status updates to Approved.
10. In Local Demo Mode, review the **Fake/Sandbox Direct Deposit** section.
11. Click **Submit Direct Deposit**.
12. Review the fake submission confirmation modal.
13. Click **Confirm Fake Submission**.
14. Confirm the fake batch status and fake batch reference are displayed.

## Common Errors

| Error | Cause | Resolution |
| --- | --- | --- |
| Payroll must be calculated before approval | The run is still Draft | Click Recalculate Payroll |
| No employees in this payroll run | No employees were selected | Create a payroll run with eligible employees |
| Gross pay must be greater than zero | No wages are present | Review hours, salary, or manual adjustments |
| Missing verified bank accounts | Direct deposit lines exist for employees without verified accounts | Add or verify bank accounts before approval |
| Fake direct deposit is available only when Local-Only Mode is enabled and real ACH submission is disabled | Local-only safety flags do not allow fake ACH | Review LimitedLiabilityMode settings |
| This payroll run already has a direct deposit batch | A fake batch was already created | Use the displayed fake batch reference; do not submit again |

## Troubleshooting

- If recalculation fails, confirm the payroll run still has a pay schedule and active employees.
- If approval is disabled, fix validation errors first.
- If approval fails, read the error message and review employee net pay and direct deposit setup.
- If fake direct deposit fails, confirm the run is Approved, the company has a fake funding account token, and each direct deposit employee has a verified fake bank token.

## Payroll, Tax, ACH, and Security Cautions

- Do not submit real ACH from this page.
- Do not submit tax filings from this page.
- Do not enter or display SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets.
- The fake/sandbox direct deposit workflow is for local prototype testing only.
- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
