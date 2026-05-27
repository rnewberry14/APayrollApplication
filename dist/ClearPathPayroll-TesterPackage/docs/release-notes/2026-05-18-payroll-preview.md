# Release Notes - Payroll Preview Feature

**Release Date**: May 18, 2026

**Version**: 2.1.0

## Feature Summary

The Payroll Preview page enables users to review calculated payroll details before approval. This critical workflow control provides visibility into gross pay, taxes, deductions, net pay, and employer costs for all employees in a payroll run.

## What's New

### Payroll Preview Page (`/payroll/preview/{id}`)

**Key Features**:
- **Run Summary Display**: Company name, pay period, pay date, and status
- **Employee Details Table**: Gross pay, deductions, taxes, and net pay per employee
- **Payroll Totals**: Aggregated payroll calculations with totals
- **Employer Cost Calculation**: Total cash required including employer taxes
- **Validation System**: 
  - Validation errors prevent approval (must be fixed)
  - Warnings indicate items to review (do not prevent approval)
- **Approval Workflow**:
  - Recalculate button to refresh from current data
  - Approve button (enabled only if no validation errors)
  - Cancel button to return without changes
- **Confirmation Modal**: Requires approval confirmation before status change
- **Error Handling**: User-friendly error messages for all common issues

### Validation Rules

**Validation Errors** (block approval):
- No employees in payroll run
- Gross pay is zero or negative
- Payroll already approved or submitted

**Warnings** (appear but don't block approval):
- Pay date is in the past (retroactive payroll)
- Total deductions exceed gross pay
- One or more employees have zero tax withholding
- **NEW**: Minimum wage violations for hourly employees
  - Regular employees: Effective hourly rate below state minimum
  - Tipped employees: Base wage + tips below state minimum requirement
  - Includes all 50 state minimum wage rates (current as of May 2026)
  - Special handling for tipped employees (federal minimum $2.13/hr)

### Security & Privacy

- **No SSNs Displayed**: Employee identification uses name and ID only
- **No Bank Details**: Account information is never displayed on preview
- **Audit Trail**: All approvals logged with user, timestamp, and payroll ID
- **Role-Based Access**: Only authorized users can approve payroll
- **Wage & Tip Privacy**: Minimum wage warnings use aggregated values; no individual tip amounts exposed

### Compliance & Labor Law

**Minimum Wage Compliance**:
- Validates against all 50 state minimum wage laws
- Supports special tipped employee rates (federal minimum $2.13/hour)
- Warns when hourly employees may be below applicable minimums
- Includes state-specific rates updated as of May 2026

**Important Notes**:
- Warnings are for review purposes; system does not block payroll with minimum wage issues
- Employers are responsible for ensuring legal compliance
- Some states have local/city minimum wages higher than state; consider implementing additional validation
- Certain employee classes may have different minimums (apprentices, students); not currently distinguished
- Tipped employee minimum wage rules vary by state; refer to state-specific resources

**Tipped Employee Handling**:
- Automatically detects tip income from earning line descriptions
- Validates that base wage + tips meets minimum wage requirement
- Federal rule: $2.13/hour minimum if tips make up difference to minimum wage
- State variations: Some states require higher minimums; refer to state DOL guidance

### Workflow Integration

**Before**: Users had no visibility into calculated payroll before submission

**After**: 
1. Payroll calculated (existing workflow)
2. Open Payroll Preview page to review all details
3. Approve payroll (status changes to "Approved")
4. Submit for direct deposit processing (existing workflow)

## Technical Changes

### New Files

- `src/ClearPathPayroll/Components/Pages/PayrollPreview.razor`: Main Blazor component

### Modified Files

- `src/ClearPathPayroll/Program.cs`: No changes (no new DI registrations required)

### Database Changes

No database schema changes. Uses existing `PayrollRun` and `PayrollRunEmployee` tables.

### API Changes

No new API endpoints. Uses existing:
- `PayrollService.GetPayrollRunByIdAsync()`
- `PayrollService.UpdatePayrollRunAsync()`
- `PayrollCalculationService.CanRecalculatePayrollRunAsync()`

## Behavioral Changes

### Payroll Approval Process

**Previous Behavior**: Approval available immediately upon calculation

**New Behavior**: Approval requires:
1. Navigation to preview page
2. Review of validation errors and warnings
3. Explicit approval confirmation via modal
4. Approval only possible if no validation errors

### Status Transitions

Approval still transitions status from Draft/Calculated → Approved (unchanged)

**New**: Approval is now intentional two-step process (button + confirmation)

## Breaking Changes

**None**. This is an additive feature. Existing payroll workflows continue to work.

## Deprecated Features

**None**.

## Known Issues

1. **Recalculate Limitation**: Recalculate button refreshes from database but doesn't accept new employee input data. Users must return to data entry page to modify hours/salary before recalculating.

2. **Large Payroll Runs**: No pagination on employee table. Payroll runs with 1000+ employees may display slowly. Workaround: None currently; large runs may require optimization in future release.

3. **Concurrent Approval**: If two managers approve same payroll simultaneously, last approval wins. No conflict detection. Workaround: Ensure only one person approves each run; implement approval queue if needed.

4. **Minimum Wage Warnings**: 
   - Warnings are informational only; not enforced at database level
   - Some states have local minimums higher than state minimum; not currently detected
   - Certain employee classes (apprentices, student workers, etc.) may have different minimums; not distinguished
   - Tipped employee rules vary by state; refer to state Department of Labor guidance
   - Salary employees: No minimum wage validation applied (assumed compliant)
   - Workaround: Payroll admin should manually verify compliance with all applicable local and state laws

## Testing

### Test Coverage

- **Component Tests**: Unit tests for validation logic, state management, and data loading
- **Integration Tests**: Full workflow tests including database save
- **Manual Testing**: Completed across browser types and user permission levels

### Test Results

- All validation rules tested for both positive and negative cases
- Modal confirmation tested for proper user confirmation flow
- Error handling tested for database failures and invalid data
- 114 total tests passing (existing tests + new validation/workflow tests)

## Migration Guide

### For End Users

No action required. The Payroll Preview page is automatically available.

**To Use**:
1. Open a payroll run in Draft or Calculated status
2. Click "Preview" to open `/payroll/preview/{id}`
3. Review employee details and totals
4. Click "Approve Payroll" (after confirming no validation errors)
5. Confirm approval in modal dialog
6. Status changes to "Approved"

### For System Administrators

**Recommended Actions**:
1. Configure role-based access for `payroll:approve_run` permission
2. Train payroll managers on new approval workflow
3. Monitor approval times to ensure timely processing
4. Review audit logs to verify approval tracking

### For Developers

**Integration Notes**:
- Component requires `PayrollService` and `PayrollCalculationService` injected
- Uses standard Blazor routing and forms
- No external dependencies beyond existing services
- Bootstrap CSS required for UI (already in project)

## Performance Impact

- **Page Load Time**: 1-2 seconds (includes DB query and render)
- **Recalculate Time**: 2-5 seconds (refresh + validation)
- **Approve Time**: 1-2 seconds (DB update + redirect)

No performance impact on other features.

## Browser Support

Tested and supported on:
- Chrome 125+
- Firefox 124+
- Safari 17+
- Edge 125+

## Accessibility

- Semantic HTML structure
- Text labels for all color-coded elements
- Focus management in modal
- Keyboard navigation supported

## Documentation

New documentation files created:
- `docs/help/payroll-preview.md`: User guide
- `docs/admin/payroll-preview-admin.md`: Administrator guide
- `docs/support/payroll-preview-troubleshooting.md`: Support troubleshooting
- `docs/technical/payroll-preview-technical.md`: Technical documentation

## Bug Fixes

This release includes no bug fixes (new feature only).

## Support & Issues

### Reporting Issues

Report any issues through support channels with:
- Payroll Run ID
- User account information
- Steps to reproduce
- Expected vs. actual behavior
- Screenshot (if applicable)

**Do not include**: SSNs, full bank account numbers, API keys

### Known Limitations

1. Recalculate requires return to data entry page for employee input changes
2. Large payroll runs (1000+) may be slow without pagination
3. No concurrent approval protection
4. Cannot export or print preview directly

## Future Enhancements

Planned for future releases:
- Batch approval (multiple payroll runs)
- PDF export capability
- Historical payroll comparison
- Approval notes/comments
- Dual approval workflow for large payroll
- Auto-approval scheduling

## Upgrade Notes

**From Previous Version**: No upgrade required. Feature is automatically available in this version.

**Database Migration**: No migration needed (uses existing tables).

**Configuration Changes**: No configuration changes required.

## Feedback

Customer feedback on this feature is welcome. Report to product team through standard channels.

## Questions?

See documentation:
- **Users**: `docs/help/payroll-preview.md`
- **Admins**: `docs/admin/payroll-preview-admin.md`
- **Support**: `docs/support/payroll-preview-troubleshooting.md`
- **Developers**: `docs/technical/payroll-preview-technical.md`

---

**Release Manager**: Development Team
**QA Sign-off**: Passed
**Product Owner Approval**: Approved
