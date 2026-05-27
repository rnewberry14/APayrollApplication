# Payroll Preview - Administrator Guide

## Feature Overview

The Payroll Preview page is a critical control point in the payroll processing workflow. It provides comprehensive visibility into calculated payroll data and enables approval for submission to direct deposit processing.

## For System Administrators

### Access Control

Restrict payroll preview access to appropriate personnel:

```
Permission: payroll:preview_run
Permission: payroll:approve_run
```

Configure role-based access:
- **Payroll Manager**: Can preview and approve own company payroll
- **Payroll Administrator**: Can preview and approve all company payroll
- **Finance Manager**: Can preview but not approve payroll
- **Employee**: No access

### Audit and Compliance

All approvals are logged in the audit trail with:
- User ID who approved
- Timestamp of approval
- Payroll run ID
- Status before and after

Access audit logs at: `/admin/audit-logs?type=PayrollApproval`

### Monitoring Payroll Runs

**Key Metrics to Track**:
- Average time from calculated to approved status
- Approval rejection rate (recalculate before approval)
- Late approvals (past pay date)

**Dashboard**: View payroll statistics at `/admin/payroll-dashboard`

## Configuration

### Validation Rules

Validation errors prevent approval. These are hard-coded business rules:

| Validation | Purpose | Bypass? |
|-----------|---------|----------|
| Must have employees | Payroll run not empty | No |
| Gross pay > $0 | Valid payroll run | No |
| Status not Approved/Submitted | Prevent double-approval | No |

### Warning Rules

Warnings appear but don't prevent approval. Review these in the application code:

- Pay date in past (retroactive payroll)
- Deductions exceed gross pay
- Employees with zero tax withholding
- Minimum wage violations (hourly employees)

**Minimum Wage Validation Details**:
- Applies to: Hourly employees only (Salary type ignored)
- Regular employees: Effective hourly rate (gross ÷ hours) must meet or exceed state minimum wage
- Tipped employees: 
  - Base wage (excluding tips) can be as low as $2.13/hr (federal minimum)
  - Total compensation (base + tips) must meet state minimum wage requirement
  - Warns if base wage is below $2.13 or total is insufficient
- State minimum wages: Defined in [MinimumWageValidator.cs](../../src/ClearPathPayroll/Components/Pages/MinimumWageValidator.cs)
- Tipped status: Detected automatically from earning line descriptions containing "tip", "gratuity", "service charge", etc.
- To modify warning logic or update state minimum wages, see [MinimumWageValidator.cs](../../src/ClearPathPayroll/Components/Pages/MinimumWageValidator.cs)

### Tax Calculation Display

The preview shows tax totals from:
- Federal income tax
- State income tax (based on residence address)
- Local income tax (if applicable)
- Social Security tax (with wage base limit notes)
- Medicare tax (including 0.9% additional)

Tax calculations come from `ITaxCalculationService`. In development, uses `FakeTaxCalculationService`.

## Operational Procedures

### Daily Payroll Approval

1. **Open Preview Page**
   - Review for validation errors
   - If errors exist, notify payroll team to correct

2. **Review Totals**
   - Compare to previous periods (should be similar)
   - Verify cash requirements with accounting

3. **Approve Payroll**
   - Confirm approval with manager if needed
   - Note any unusual items in payroll notes

4. **Monitor Processing**
   - Check DirectDepositBatch status
   - Verify settlement by end of business day

### Handling Common Issues

**Issue: Approval button disabled**
- Likely cause: Validation errors present
- Solution: Fix all red error messages
- Escalate to engineering if errors are incorrect

**Issue: Wrong amounts displayed**
- Likely cause: Employee hours/salary incorrect
- Solution: Recalculate from data entry page
- Verify tax settings if still incorrect

**Issue: Cannot approve retroactive payroll**
- Behavior: "Pay date in past" is a warning, not an error
- Procedure: Warnings do not prevent approval
- Confirm retroactive approval with payroll manager

### Payroll Approval Checklist

Before approving, verify:
- [ ] All employees expected are present
- [ ] No validation errors (red messages)
- [ ] Gross pay totals match expectations
- [ ] Taxes appear reasonable
- [ ] No unusual deductions
- [ ] Pay date is correct
- [ ] Cash requirements budgeted and available

## Technical Details

### Component Location
- **File**: `src/ClearPathPayroll/Components/Pages/PayrollPreview.razor`
- **Route**: `/payroll/preview/{id}`
- **Services Used**: `PayrollService`, `PayrollCalculationService`

### Data Display

The page displays:
- `PayrollRun` aggregate with all employee details
- `PayrollRunEmployee` collection with earnings, deductions, taxes
- Calculated totals for cash flow planning

No SSNs or full bank account numbers are displayed.

### Approval Workflow

```
Draft → (Preview) → Approve → Approved → (Submit) → Submitted
                   ↓
              Recalculate
```

Only Draft and Calculated status payroll can be approved.

### API Integration

**Approving Payroll**:
- Updates `PayrollRun.Status` to `Approved`
- Calls `PayrollService.UpdatePayrollRunAsync()`
- Creates audit log entry with user ID and timestamp

**Recalculating**:
- Checks `PayrollCalculationService.CanRecalculatePayrollRunAsync()`
- Returns error if status not Draft/Calculated
- Re-fetches data from database

## Direct Deposit Integration

After approval, payroll can be submitted for direct deposit:

1. Create `DirectDepositBatch` referencing this `PayrollRunId`
2. Create `DirectDepositItem` for each employee
3. Call `IAchPaymentService.SubmitBatchAsync()`

Do not create direct deposit batches until payroll is Approved status.

## Security Considerations

### Data Protection
- No raw SSNs stored or displayed
- Bank account info only shown as tokens + Last4
- API keys never logged or displayed

### Access Audit
- Every approval logged to audit trail
- Track who approved payroll and when
- Review unexpected approvals immediately

### Approval Authority
- Enforce single approver or dual control for large payroll
- Never approve own payroll (segregation of duties)
- Require manager approval for retroactive changes

## Support and Escalation

### When to Escalate to Engineering
- Page will not load
- Validation/warning logic appears incorrect
- Amounts don't match tax API responses
- Approval button non-functional despite no errors
- Performance issues with large payroll runs (1000+ employees)

### Information to Provide to Engineering
- Payroll run ID
- Error message/screenshot
- Steps to reproduce
- Expected vs. actual result

**Do not provide**: Customer SSNs, full bank accounts, or live API responses.
