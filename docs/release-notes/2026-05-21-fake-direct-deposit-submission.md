# 2026-05-21 - Fake Direct Deposit Submission

## Summary

Added a fake/sandbox direct deposit submission flow for approved payroll runs.

## Changes

- Added Submit Direct Deposit button on Payroll Preview for Approved payroll runs.
- Added confirmation modal before fake submission.
- Added batch status and fake external batch reference display.
- Prevented duplicate submission.
- Blocked non-fake ACH services from this workflow.
- Added user, admin, support, and technical documentation.

## Security Notes

- No real ACH submission is allowed.
- Full bank account numbers and routing numbers are not shown.
- Fake/sandbox direct deposit is clearly labeled in the UI.
