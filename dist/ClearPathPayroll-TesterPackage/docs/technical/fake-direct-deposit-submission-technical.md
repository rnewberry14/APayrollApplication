# Fake Direct Deposit Submission - Technical Notes

## Overview

Fake Direct Deposit Submission is surfaced from `PayrollPreview.razor` when a payroll run is Approved. It calls `DirectDepositSubmissionService.SubmitFakePayrollRunDirectDepositAsync`.

## Implementation Details

- UI: `src/ClearPathPayroll/Components/Pages/PayrollPreview.razor`
- Service: `src/ClearPathPayroll/Services/DirectDepositSubmissionService.cs`
- ACH implementation: `FakeAchPaymentService` only

The fake submission method blocks execution if the injected ACH service is not `FakeAchPaymentService`. It always calls the existing direct deposit submission method with `sandboxMode: true`.

## Who Can Use It

Users with approved payroll and sandbox direct deposit submission permissions.

## Required Permissions

- Read payroll run.
- Read employee net pay lines.
- Read employee bank verification status.
- Read company funding account.
- Create direct deposit batch and items.

## Common Errors and Troubleshooting

- Non-fake ACH configured: verify DI registration for `IAchPaymentService`.
- Missing company funding account: add an active sandbox funding account.
- Missing employee bank account: add an active verified sandbox account.
- Duplicate submission: query `DirectDepositBatches` for the payroll run.

## Payroll, Tax, ACH, and Security Cautions

- Do not transmit real bank data.
- Do not show full bank account numbers.
- Do not show routing numbers.
- Do not use production ACH.
- Do not submit tax filings.
