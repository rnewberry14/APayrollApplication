Payroll Approval - Technical Documentation

What it does
- Service: `PayrollApprovalService` performs validations, writes an `AuditLogEntry`, and transitions the payroll run to `Approved` using `PayrollService.UpdatePayrollStatusAsync`.

Who can use it
- Backend services and UI controllers/components (e.g., `PayrollPreview.razor`) should call `PayrollApprovalService.ApprovePayrollRunAsync(payrollRunId, userId)`.

Key validations
- Payroll run must be in `PayrollStatus.Calculated`.
- Payroll run must contain employees and have `TotalGrossPay > 0`.
- Every employee with a `NetPayLine.PaymentMethod == "DirectDeposit"` must have an active, verified `EmployeeBankAccount`.

Behavior
- On success: creates `AuditLogEntry` with `EventType=PayrollRunApproved`, calls `PayrollService.UpdatePayrollStatusAsync(...)`, and returns true.
- On failure: throws `InvalidOperationException` with descriptive message.
- After approval: payroll run is locked; `PayrollCalculationService.CanRecalculatePayrollRunAsync` will return false until run is `Voided`.

Data models involved
- `PayrollRun`, `PayrollRunEmployee`, `NetPayLine`, `Employee`, `EmployeeBankAccount`, `AuditLogEntry`.

DI registration
- Service registered in `Program.cs` with `AddScoped<PayrollApprovalService>()`.

Unit tests
- Tests added in `tests/ClearPathPayroll.Tests/PayrollApprovalServiceTests.cs` covering success and failure scenarios using an EF Core in-memory database.

Notes for developers
- Keep audit entries minimal and avoid logging sensitive bank/account details.
- Use `PayrollService.UpdatePayrollStatusAsync` to maintain status transition rules and approved metadata.
