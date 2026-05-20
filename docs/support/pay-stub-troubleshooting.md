Pay Stub - Support Troubleshooting

What it does
- Shows a printable pay stub for an employee in a payroll run.

Common issues
- "Pay stub not found": The payroll run or employee is invalid.
- Missing earnings, taxes, or deductions: The payroll run has not been fully calculated for this employee.
- Direct deposit last 4 missing: Employee does not have an active verified bank account.

Support steps
1. Verify the payroll run and employee IDs.
2. Confirm the payroll run has employee calculation data and net pay lines.
3. Check `EmployeeBankAccounts` for `VerificationStatus=Verified` and `IsActive=true`.
4. If the pay stub fails to load, confirm the employee is part of the payroll run.

Escalation
- If the page fails with an exception, check the server logs and the `PayrollRunEmployees` and `EmployeeBankAccounts` records.

Security notes
- Only use last 4 digits of SSN for identification.
- Do not ask for or display full bank account numbers.
