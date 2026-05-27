# Check Calculation Preview

Date: 2026-05-26

## Added

- Check calculation preview workflow at `/payroll/check-calculation/{payrollRunId}`.
- Editable regular hours, overtime hours placeholder, additional earnings, manual deductions, reimbursements, and notes.
- Recalculate Check and Recalculate All actions.
- Line-item persistence for earnings, deductions, employee taxes, employer taxes, and net pay.
- Audit log entries for recalculation.
- Documentation and tests.

## Safety Notes

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. This workflow does not approve payroll, submit ACH, submit tax filings, or display full SSNs or full bank account numbers. Approved payroll runs are blocked from recalculation on this page.
