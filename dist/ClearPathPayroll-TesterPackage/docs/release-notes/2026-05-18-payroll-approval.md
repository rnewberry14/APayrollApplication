Payroll Approval - 2026-05-18

Summary
- Adds `PayrollApprovalService` to validate, approve, and lock payroll runs.

What's new
- Approve payroll runs only when status is `Calculated` and no blocking errors exist.
- Validates direct deposit employees have verified, active bank accounts.
- Records approver and timestamp on `PayrollRun`.
- Creates `AuditLogEntry` with `EventType=PayrollRunApproved`.
- Prevents recalculation after approval; use `VoidPayrollRun` for corrective actions.

Migration notes
- No DB schema changes required.

Testing
- Unit tests added: `PayrollApprovalServiceTests` (approval success, not calculated, bank verification failure).

Security
- Audit logs do not contain raw bank account or SSN data.

Known limitations
- Local/city minimum wage checks and other warnings remain informational and do not block approval.
