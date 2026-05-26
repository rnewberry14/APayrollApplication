# 2026-05-21 - Payroll Preview Calculation and Approval

## Summary

Wired Payroll Preview actions to the payroll calculation, approval, and fake/sandbox direct deposit services.

## Changes

- Recalculate Payroll now calls `PayrollCalculationService`.
- Approve Payroll now calls `PayrollApprovalService`.
- Added a safe placeholder user ID until authentication is implemented.
- Approval remains behind a confirmation modal.
- Approval refreshes the page and shows Approved status.
- Approval is disabled when validation errors exist or the run is not Calculated.
- Added Local Demo Mode banner and no-advice disclaimer coverage.
- Added fake/sandbox direct deposit submission after approval.
- Fake direct deposit is enabled only when Local-Only Mode is enabled and real ACH submission is disabled.
- Fake direct deposit displays batch status and fake external batch reference after submission.
- Duplicate fake direct deposit submission is blocked.
- Added documentation for users, admins, support, and technical maintainers.

## Cautions

- Real direct deposit is not submitted by approval or preview.
- Fake/sandbox direct deposit is for local prototype testing only.
- Tax filings are not submitted by approval.
- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- SSNs and full bank account numbers are not shown.
