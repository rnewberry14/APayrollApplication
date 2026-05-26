# Payroll Preview Calculation and Approval - Admin Guide

## What the Feature Does

Payroll Preview now calls the calculation service for recalculation, the approval service for approval, and the direct deposit submission service for fake/sandbox direct deposit after approval. The direct deposit action is available only in Local-Only Mode when real ACH submission is disabled.

## Who Can Use It

Payroll administrators and payroll managers with payroll calculation and approval permissions.

## Required Permissions

- Payroll run read access.
- Payroll calculation access.
- Payroll approval access.
- Local prototype fake direct deposit access.
- Audit log create access.

## Admin Steps

1. Create a draft payroll run.
2. Open `/payroll/preview/{PayrollRunId}`.
3. Click **Recalculate Payroll**.
4. Review totals, warnings, and validation messages.
5. Click **Approve Payroll**.
6. Confirm approval in the modal.
7. Verify the run status is Approved.
8. Verify the Local Demo Mode banner is visible.
9. Click **Submit Direct Deposit** in the Fake/Sandbox Direct Deposit section.
10. Confirm fake submission in the modal.
11. Verify a fake batch status and fake batch reference are displayed.

## Common Errors

- Draft run cannot be approved until recalculated.
- Submitted or completed payroll runs cannot be recalculated.
- Approval may fail if direct deposit employees do not have verified bank accounts.
- Fake direct deposit is blocked if `LimitedLiabilityMode.Enabled` is false or `AllowRealAchSubmission` is true.
- Duplicate fake direct deposit submission is blocked once a submitted or settled batch exists.

## Troubleshooting

1. Confirm the payroll run status is Draft or Calculated before recalculation.
2. Confirm the run has selected employees.
3. Review audit log entries for recalculation and approval.
4. Confirm no ACH batch or tax filing records were created by approval alone.
5. Confirm fake direct deposit creates only fake/sandbox batch records with a fake external reference.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not submit real direct deposit from approval or preview.
- Do not submit tax filings from approval.
- Keep `AllowRealAchSubmission` set to false for local prototype testing.
- Do not expose SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets.
