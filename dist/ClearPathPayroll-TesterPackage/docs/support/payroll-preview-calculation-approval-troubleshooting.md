# Payroll Preview Calculation and Approval - Troubleshooting

## What the Feature Does

The Payroll Preview page recalculates payroll through `PayrollCalculationService`, approves payroll through `PayrollApprovalService`, and can create a fake/sandbox direct deposit batch through `DirectDepositSubmissionService` after approval in Local-Only Mode.

## Who Can Use It

Payroll managers, payroll administrators, and support users with appropriate permissions.

## Required Permissions

- View payroll run details.
- Recalculate payroll runs.
- Approve calculated payroll runs.
- Submit fake/sandbox direct deposit in local prototype mode.

## Common Errors

| Symptom | Likely Cause | Troubleshooting Step |
| --- | --- | --- |
| Approve button is disabled | Validation errors exist or run is not Calculated | Click Recalculate Payroll and resolve validation errors |
| Recalculate fails | Run status is not Draft or Calculated | Check payroll run status |
| Approval fails | Direct deposit bank verification requirement failed | Review employee bank verification status without sharing full account data |
| Status does not change | Save failed or service threw an error | Review the page error and application logs |
| Submit Direct Deposit is disabled | Local-Only Mode is off, real ACH is enabled, the run is not Approved, or a batch already exists | Review `LimitedLiabilityMode` and the displayed batch section |
| Fake direct deposit fails | Missing fake funding account or missing verified fake employee bank token | Seed demo data again or review token records without displaying full bank data |

## Troubleshooting Steps

1. Confirm the payroll run ID is valid.
2. Confirm the page shows employees and gross pay.
3. Click **Recalculate Payroll**.
4. Confirm the success message appears.
5. Confirm the status is Calculated.
6. Click **Approve Payroll** and then **Confirm Approval**.
7. Confirm the status is Approved.
8. Click **Submit Direct Deposit** and then **Confirm Fake Submission**.
9. Confirm the fake batch reference appears and no routing numbers or full bank account numbers are shown.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not submit real direct deposit from this page.
- Do not submit tax filings from this page.
- Do not request SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets.
