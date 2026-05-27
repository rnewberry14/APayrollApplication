# Payroll Preview - User Guide

## Overview

The Payroll Preview page allows you to review all payroll calculations before approval. It displays gross pay, taxes, deductions, net pay, and employer costs for all employees in a payroll run.

## What the Page Does

- **Displays Run Summary**: Company name, pay period, pay date, and status
- **Shows Employee Details**: Individual gross pay, deductions, taxes, and net pay for each employee
- **Calculates Totals**: Aggregated payroll totals and total cash required
- **Validates Data**: Displays warnings and errors before approval
- **Allows Actions**: Recalculate, approve, or cancel the payroll run

## Who Can Use It

- **Payroll Managers**: Review and approve payroll runs for their company
- **Payroll Administrators**: Manage payroll across multiple companies
- **Finance Managers**: Monitor payroll costs and cash flow requirements

## Required Permissions

- **View Payroll**: Must have permission to view payroll run details
- **Approve Payroll**: Must have payroll administration or manager role
- **Recalculate Payroll**: Must have payroll administration role

## Step-by-Step Instructions

### Accessing the Payroll Preview

1. Navigate to the Payroll section of ClearPath Payroll
2. Click on a payroll run in Draft or Calculated status
3. Select "Preview Payroll" to open the preview page
4. The page will load and display all payroll details

### Reviewing Payroll Details

1. **Check the Run Summary** at the top:
   - Verify company name is correct
   - Confirm pay period and pay date
   - Check current status

2. **Review Employee List**:
   - Verify all expected employees are present
   - Check gross pay amounts match expectations
   - Review deduction and tax amounts

3. **Check Totals**:
   - Verify total gross pay matches sum of employee amounts
   - Confirm total taxes are reasonable
   - Review total net pay amount
   - **Important**: Note the "Total Cash Required" = Net Pay + Employer Taxes

### Understanding Validation Errors

Validation errors (shown in red) prevent approval. Address all errors before approving:

- **No employees**: Add employees to the payroll run
- **Zero gross pay**: Ensure all employees have hours or salary entered
- **Already approved/submitted**: Cannot re-approve; check for duplicate runs

### Understanding Warnings

Warnings (shown in yellow) do not prevent approval but indicate items to review:

- **Past pay date**: Confirm the date is intentional for retroactive payroll
- **Deductions exceed gross pay**: Review deduction amounts
- **No tax withholding**: Confirm employees' tax election status
- **Minimum wage violations**: Hourly employee's effective wage is below state minimum wage
  - For regular employees: Gross pay ÷ hours worked must meet or exceed state minimum wage
  - For tipped employees: Base wage + tips must meet state minimum wage
    - Tipped employees can have lower base wages (federal minimum $2.13/hr) if tips make up the difference
    - Warning appears if base wage is below federal tipped minimum or if total compensation is insufficient

### Recalculating Payroll

1. Click the **Recalculate Payroll** button
2. The system will refresh calculations
3. Review the updated amounts
4. If changes are needed, return to data entry page

### Approving Payroll

1. Review all employee details and totals
2. Verify no validation errors are shown
3. Click **Approve Payroll** button
4. A confirmation dialog will appear showing:
   - Company name
   - Pay period
   - Total net pay
   - Total employer taxes
   - Warning about the action being difficult to undo
5. Click **Confirm Approval** to proceed
6. Payroll status changes to "Approved"
7. You can now submit for direct deposit processing

### Canceling

1. Click **Cancel** to return to the payroll list without making changes

## Security and Privacy

- **No SSNs Displayed**: Employee Social Security Numbers are never shown
- **No Full Bank Account Numbers**: Only the last 4 digits of accounts are visible (if displayed)
- **Audit Trail**: All approval actions are logged for compliance
- **Role-Based Access**: Only authorized users can approve payroll

## Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| "No employees in this payroll run" | Payroll run created but no employees added | Add employees to payroll run or check pay schedule |
| "Gross pay must be greater than zero" | Employees have $0 hours/salary | Enter hours or salary for all employees |
| "Already been approved" | Trying to re-approve submitted payroll | Cannot re-approve; create a new payroll run |
| "Cannot recalculate" in this status | Payroll in Submitted/Completed status | Can only recalculate Draft or Calculated runs |
| Button is grayed out | Validation errors present | Fix all red error messages first |

## Troubleshooting

### Page Won't Load
- Check internet connection
- Clear browser cache
- Verify payroll run ID is valid
- Contact support if error persists

### Data Appears Incorrect
- Verify employee hours/salary were entered correctly
- Check tax withholding elections for employees
- Review deduction setup
- Click "Recalculate Payroll" to refresh
- If issue persists, return to data entry page and correct source data

### Cannot Approve Payroll
- Verify you have payroll approval permission
- Check for validation errors (red messages)
- Confirm payroll run status is Draft or Calculated
- Ensure all employees have gross pay > $0

### Amounts Don't Match Expectations
- Verify tax calculation settings
- Check withholding allowances
- Review deduction amounts
- Confirm pay frequency matches payroll schedule
- Contact accounting if still unclear

## When to Contact Support

Contact ClearPath Payroll Support if you encounter:
- Page will not load or crashes
- Data appears corrupted or incorrect
- Calculations significantly differ from previous periods
- Approval button remains disabled despite no visible errors
- Need to modify already-approved payroll

**Do NOT share**: Full SSNs, full bank account numbers, or any customer financial account details.

## Tips and Best Practices

1. **Review in Sequence**: Check summary, employees, then totals
2. **Verify Gross Pay First**: Ensure all hours/salary are correct before reviewing taxes
3. **Check Warnings**: Don't ignore yellow warnings—they often indicate data quality issues
4. **Use Recalculate**: If data was updated, click Recalculate before approving
5. **Print or Screenshot**: Keep a record of previewed payroll for audit purposes
6. **Timely Review**: Review and approve payroll promptly to maintain processing schedules
