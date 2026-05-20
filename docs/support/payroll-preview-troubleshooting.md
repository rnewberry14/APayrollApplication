# Payroll Preview - Support Troubleshooting Guide

## Overview

This guide helps support staff assist customers with the Payroll Preview page and payroll approval process.

## Pre-Call Checklist

Before contacting a customer, confirm:
- [ ] You can reproduce the issue in a test environment
- [ ] You have the payroll run ID (not customer account details)
- [ ] You understand what the customer expected to happen
- [ ] You have logged into the appropriate customer instance

## Common Customer Issues and Solutions

### "Page Won't Load"

**Symptoms**: Blank page or spinner that never completes

**Troubleshooting Steps**:
1. Ask: "What browser are you using?" (Test in Chrome first)
2. Ask: "When did it last work?" (Identify if new issue)
3. Ask customer to: Clear browser cache
4. Ask customer to: Hard refresh (Ctrl+Shift+R or Cmd+Shift+R)
5. Ask customer to: Try a different browser
6. If still not loading:
   - Check payroll run ID: `/payroll/preview/{id}`
   - Verify payroll run exists in database
   - Check server logs for errors
   - Escalate to engineering with run ID and timestamp

**Common Fix**: Browser cache issue. Customer clears cache and problem resolves.

### "Validation Error: 'No employees in this payroll run'"

**Symptoms**: Red error message prevents approval

**Troubleshooting**:
1. Ask: "Did you add all employees to the payroll run?"
2. Verify in database:
   ```sql
   SELECT COUNT(*) FROM PayrollRunEmployees WHERE PayrollRunId = {id}
   ```
3. If count is 0:
   - Customer must return to data entry page
   - Verify pay schedule includes all active employees
   - Add any missing employees
   - Click "Recalculate Payroll"
   - Return to preview
4. If count > 0 but error still shown:
   - Escalate to engineering (data loading issue)

**Expected Outcome**: Error resolves after employees are added.

### "Validation Error: 'Gross pay must be greater than zero'"

**Symptoms**: Red error message, cannot approve

**Troubleshooting**:
1. Ask: "Did you enter hours or salary for all employees?"
2. Have customer return to data entry page
3. Verify each employee has:
   - Hours worked for hourly employees
   - Salary amount for salaried employees
4. Click "Recalculate Payroll"
5. Return to preview and retry

**Expected Outcome**: Error clears once all employees have gross pay entered.

### "Approval Button is Grayed Out"

**Symptoms**: Button appears disabled/unclickable

**Causes**:
1. Validation errors present (scroll up for red messages)
2. System processing (shows spinner)
3. Permission issue (user cannot approve)

**Troubleshooting**:
1. Ask: "Do you see any red error messages?"
   - If yes: Fix validation errors first
2. Ask: "Is there a loading spinner?" 
   - If yes: Wait for processing to complete
3. Verify permission:
   - Check if user has `payroll:approve_run` permission
   - Check user's role (must be Manager or Admin)
   - If permission issue: Escalate to account manager

**Do not provide**: Customer credentials to verify permissions.

### "Wrong Amount Displayed"

**Symptoms**: Gross pay, taxes, or net pay appears incorrect

**Troubleshooting**:
1. Ask: "What amount did you expect?"
2. Ask: "Did you verify the hours/salary were entered correctly?"
3. Have customer:
   - Return to data entry page
   - Verify all hours and salary amounts
   - Verify deduction setup
   - Click "Recalculate Payroll"
4. Return to preview to see updated amounts
5. If still incorrect:
   - Check tax withholding elections for employees
   - Verify filing status
   - Check for recent payroll configuration changes

**If Amount Differs from Tax API**:
- This is development environment using `FakeTaxCalculationService`
- Placeholder tax rates: Federal 12%, State 5%, SS 6.2%, Medicare 1.45%
- In production, real tax API provides actual rates
- Do not correct for testing

### "Cannot Approve: 'Already been approved'"

**Symptoms**: Error message appears when trying to approve

**Explanation**: This payroll run was already approved

**Solution**:
1. Verify with customer: "Did someone else approve this payroll?"
2. If duplicate approval: Customer should cancel and check for existing approved run
3. If need to modify approved payroll:
   - Cannot modify approved runs
   - Must void and create new payroll run
   - Escalate to engineering for void procedure

### "Pay Date in Past Warning"

**Symptoms**: Yellow warning message about retroactive payroll

**Explanation**: This is normal for retroactive payroll runs

**Solution**:
1. Confirm with customer: "Is this intentional retroactive payroll?"
   - If yes: Warning is expected, proceed with approval
   - If no: Have customer correct pay date and recalculate
2. No action needed if intentional

### "Deductions Exceed Gross Pay Warning"

**Symptoms**: Yellow warning message

**Cause**: Total deductions > gross pay (results in negative net pay)

**Solution**:
1. Have customer review deduction setup
2. Verify:
   - Deduction amounts entered correctly
   - No duplicate deductions
   - Deduction setup matches company policy
3. Correct deduction amounts or gross pay
4. Click "Recalculate Payroll"
5. Warning should clear

**Do not ignore**: This often indicates data entry error.

### "Employee(s) Have No Tax Withholding Warning"

**Symptoms**: Yellow warning message

**Explanation**: Some employees have $0 tax withholding

**Verification Steps**:
1. Check if employee requested zero withholding (W-4 form)
2. Verify employee type:
   - Contractors: May legitimately have zero withholding
   - Employees: Should typically have withholding
3. If intentional: Warning can be ignored
4. If error: Have customer correct tax setup and recalculate
### Minimum Wage Warnings

**Symptoms**: Yellow warning message about hourly rate being below minimum wage

**Message Examples**:
- "[Employee Name]: Effective hourly rate ($X.XX/hr) is below [STATE] minimum wage of $Y.YY/hr."
- "[Employee Name]: Base wage + tips ($X.XX) falls short of [STATE] minimum wage requirement."
- "[Employee Name]: Base wage of $X.XX/hr falls below federal tipped employee minimum."

**Explanation**: Hourly employee's calculated pay is below applicable minimum wage

**For Regular (Non-Tipped) Employees**:
1. Verify hours worked were entered correctly
2. Verify gross pay amount
3. Calculate: Gross Pay ÷ Hours = Effective Hourly Rate
4. Compare to state minimum wage (see table below for state rates)
5. If rate is below minimum:
   - Increase gross pay, or
   - Reduce hours, or
   - Verify this isn't a data entry error

**For Tipped Employees**:
1. Base wage (without tips) can be as low as $2.13/hr (federal tipped minimum)
2. Verify tips were entered and included in gross pay
3. Verify Base Wage + Tips ≥ State Minimum Wage requirement
4. If base wage is extremely low ($2.13 or below per hour):
   - Confirm this is intentional (tipped position)
   - Verify tip amounts are reasonable and reported
5. If total compensation is still below minimum:
   - Increase base wage or tips, or
   - Verify tip amounts reported by employee

**State Minimum Wage Reference** (as of May 2026):
| State | Minimum Wage |
|-------|--------------|
| AK | $11.73 |
| AZ | $15.00 |
| AR | $11.00 |
| CA | $16.50 |
| CO | $14.42 |
| CT | $15.69 |
| DE | $13.25 |
| FL | $13.00 |
| HI | $14.00 |
| IL | $14.00 |
| MA | $15.00 |
| MD | $15.13 |
| MI | $12.00 |
| MN | $12.85 |
| MO | $12.30 |
| MT | $12.30 |
| NE | $14.00 |
| NJ | $15.13 |
| NM | $12.00 |
| NV | $12.00 |
| NY | $15.00 |
| OR | $15.45 |
| RI | $15.00 |
| SD | $14.00 |
| VT | $14.67 |
| VA | $12.00 |
| WA | $16.28 |
| Other states | $7.25 (Federal) |

Note: Rates shown are as of May 2026 and may change. Verify current rates with official sources.

**When to Escalate**:
- Customer disputes the warning (believes they're paying correctly)
- Warning appears for salaried employees (shouldn't happen)
- Unable to resolve by adjusting wages/tips
- Questionable whether position qualifies for tipped minimum wage
## When to Escalate to Engineering

**Escalate if**:
- Page crashes or shows error message
- Validation logic seems wrong (e.g., allows invalid data)
- Approval fails with database error
- Amounts don't match calculation service
- Performance is slow (> 10 seconds to load)
- Reproducible after cache clear and browser restart

**Information to Include**:
- Payroll run ID
- Customer name and account ID
- Steps to reproduce
- Screenshot if applicable
- Browser/OS information
- Error message (if any)

**Do NOT include**:
- Customer SSNs
- Full bank account numbers
- API keys or secrets
- Customer employee data

## Payment Processing After Approval

**Expected Workflow**:
1. Payroll Preview shows all details
2. Customer approves → Status changes to "Approved"
3. Customer proceeds to Direct Deposit page
4. Customer submits batch for ACH processing
5. Status changes to "Submitted"

**If Approval but Not Submitted**:
- Ask: "Did you proceed to the next step?"
- Direct customer to Direct Deposit/Submission page
- Help them complete batch submission

**If Submitted but Not Processing**:
- Check DirectDepositBatch status
- Look for return codes (ACH errors)
- Verify funding account is active
- May take 1-3 business days to settle

## Frequently Asked Questions

**Q: Can I modify amounts after approval?**
A: No, cannot modify approved payroll. Must void and create new run.

**Q: How do I void an approved payroll?**
A: Contact engineering or escalate. Void procedure requires special authorization.

**Q: Why does preview show different taxes than last period?**
A: Tax rates may change, withholding elections may have changed, gross pay is different. Review W-4 and tax settings.

**Q: Can I export or print this payroll?**
A: Screenshot feature in most browsers. For formal reports, escalate to product team.

**Q: Why is total cash required different from net pay?**
A: Total cash = Net Pay + Employer Taxes. Employer taxes are company responsibility, not deducted from employee pay.

## Security Reminders

**Never Ask Customer For**:
- Full Social Security Number
- Full bank account numbers
- Routing numbers
- API keys or credentials
- Passwords

**What to Ask For Instead**:
- Payroll run ID (for lookup)
- Last 4 of account (if referenced by customer)
- Company name or account ID
- Pay period dates

**If Customer Offers Sensitive Data**:
- Immediately tell them: "Please don't share that information through support channels"
- Use tokenized identifiers (run ID, last 4) instead
- Do not log, screenshot, or store sensitive data
- Document if you see sensitive data in logs (escalate to security)

## Escalation Process

1. **Attempt troubleshooting** using steps above
2. **Gather reproduction steps** and error details
3. **Check logs** for any system errors
4. **Document issue** in support ticket with full context
5. **Escalate to engineering** with ticket and test case
6. **Follow up** with customer once resolved

## Resources

- **Feature Guide**: `/docs/help/payroll-preview.md`
- **Admin Guide**: `/docs/admin/payroll-preview-admin.md`
- **Technical Details**: `/docs/technical/payroll-preview-technical.md`
- **Release Notes**: `/docs/release-notes/`
- **API Status**: Check internal wiki for tax/ACH API status

