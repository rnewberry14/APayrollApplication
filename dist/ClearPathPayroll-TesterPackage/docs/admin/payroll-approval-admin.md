Payroll Approval - Admin Guide

What it does
- Admins can approve calculated payroll runs, lock runs, and review audit logs of approvals.

Who can use it
- Administrators and Payroll Managers.

Step-by-step (Admin)
1. Review the payroll run and confirm that calculations are complete.
2. Verify all direct deposit accounts are verified for employees.
3. From the admin UI, approve the run or review the approval queue.
4. To revert an approval, use the Void Payroll Run workflow (admin-only).

Required permissions
- `Administrator` role required for voiding approvals.

Common errors & fixes
- Approval blocked because run is not Calculated: Re-run calculations.
- Missing verified bank accounts: Verify employee bank accounts via bank verification service.

Operational notes
- Approval creates an `AuditLogEntry` with `EventType=PayrollRunApproved`.
- Approved runs cannot be recalculated; use `VoidPayrollRun` for corrective action.
- Ensure backups and retention policies for audit logs.

Security & Compliance
- Never log full routing or account numbers. Use tokenized storage.
- Approvals should be timestamped in UTC for audit consistency.
