# Payroll Preview Calculation and Approval - Technical Notes

## Overview

`PayrollPreview.razor` uses `PayrollCalculationService` for recalculation, `PayrollApprovalService` for approval, and `DirectDepositSubmissionService` for fake/sandbox direct deposit after approval.

## Implementation Details

- Page: `src/ClearPathPayroll/Components/Pages/PayrollPreview.razor`
- Calculation method: `PayrollCalculationService.RecalculatePayrollRunAsync`
- Approval method: `PayrollApprovalService.ApprovePayrollRunAsync`
- Fake direct deposit method: `DirectDepositSubmissionService.SubmitFakePayrollRunDirectDepositAsync`
- Local safety options: `LimitedLiabilityModeOptions`
- Placeholder user ID: `local-prototype-user`

Recalculation processes employees already attached to the payroll run. It derives hourly values and manual gross pay adjustments from existing earning lines, recalculates taxes/deductions/net pay, updates totals, and changes Draft runs to Calculated.

Approval keeps the confirmation modal, validates status and totals, creates approval audit logs, and updates the run to Approved. It does not submit direct deposit or tax filings.

The fake/sandbox direct deposit button is enabled only when:

- The payroll run is Approved.
- No direct deposit batch already exists for the run.
- `LimitedLiabilityMode.Enabled` is true.
- `LimitedLiabilityMode.AllowRealAchSubmission` is false.

The fake submission workflow calls `SubmitFakePayrollRunDirectDepositAsync`, which requires `FakeAchPaymentService`. It displays the fake batch status and fake external batch reference after submission.

## Who Can Use It

Users with payroll review, calculation, and approval access.

## Required Permissions

- Read payroll run and employee payroll details.
- Recalculate payroll runs.
- Approve payroll runs.
- Write audit log entries.

## Common Errors and Troubleshooting

- Run not found: verify the route payroll run ID.
- Cannot recalculate: verify status is Draft or Calculated.
- Cannot approve: verify status is Calculated and validation errors are clear.
- Direct deposit verification error: verify active employee bank verification status without exposing full bank data.
- Cannot submit fake direct deposit: verify Local-Only Mode is enabled, real ACH is disabled, the run is Approved, and no submitted or settled batch already exists.
- Missing bank account error: verify fake tokenized bank records exist and are marked Verified.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not submit real ACH or direct deposit from approval or preview.
- Do not submit tax filings from approval.
- Do not log or display SSNs, full bank account numbers, routing numbers, employer tax IDs, or API secrets.
