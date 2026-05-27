# Check Calculation Preview

ClearPath Payroll provides local software tools for payroll calculation, reporting, and data organization. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## What This Feature Does

The check calculation preview page lets users review employee checks before payroll approval. Users can edit regular hours, overtime hours placeholder, additional earnings, manual deductions, reimbursements, and notes, then recalculate one check or all checks.

## Who Can Use It

Users with local payroll run access in the ClearPath Payroll application.

## Steps

1. Open `/payroll/check-calculation/{payrollRunId}`.
2. Review the payroll run header.
3. Edit check input fields for each employee.
4. Select **Recalculate Check** for one employee check.
5. Select **Recalculate All** to recalculate all employee checks.
6. Review gross pay, deductions, employee taxes, employer taxes, reimbursements, and net pay.
7. Resolve any errors that block approval.
8. Select **Continue to Payroll Preview / Approval** when ready to review the separate approval confirmation workflow.

## Required Permissions

Local payroll access for the selected payroll run.

## Common Errors

- Payroll run has no employee checks.
- Payroll run is already approved, submitted, or completed.
- Check input amounts are negative.
- Net pay is negative after deductions and taxes.

## Cautions

This page does not approve payroll, submit ACH, submit tax filings, or transmit bank data. Approved payroll runs are locked from recalculation on this page.
