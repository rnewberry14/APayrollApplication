Pay Stub - Technical Documentation

What it does
- `PayStubService` constructs a printable pay stub model from payroll run employee data.
- The pay stub page displays company, employee, pay period, earnings, taxes, deductions, net pay, and direct deposit information.

Who can use it
- Developers building payroll reporting and pay stub views.

Implementation details
- `PayStubService.GetPayStubAsync(int payrollRunId, int employeeId)` loads `PayrollRunEmployee` data with related `Employee`, `PayrollRun`, `Company`, earnings, deductions, taxes, and net pay lines.
- It returns `PayStubModel` with masked SSN, direct deposit last 4, and placeholder YTD totals.
- The `PayStub.razor` page uses route `/paystub/{PayrollRunId:int}/{EmployeeId:int}`.
- It uses `window.print()` via JavaScript interop for a print-friendly output.

Security and privacy
- Only `SSNLast4` is exposed; full SSN data is never shown.
- No raw bank account or routing data is displayed.
- Direct deposit display is limited to bank account last 4 digits.

Testing
- `PayStubServiceTests` covers pay stub generation, masked SSN, direct deposit visibility, and missing data behavior.
