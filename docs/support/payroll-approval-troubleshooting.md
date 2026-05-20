Payroll Approval - Support Troubleshooting

What it does
- Approve a calculated payroll run; records approver and creates audit log.

Common customer issues
- "Must be in Calculated status": Customer attempted approval before running calculations.
- "No employees in payroll run": The run has no processed employees; instruct customer to run payroll calculations.
- "Direct deposit employees missing verified bank accounts": Employee bank accounts are not verified or inactive.

Support steps
1. Ask for payroll run ID and approver username.
2. Check `PayrollRuns` status and totals.
3. Check `PayrollRunEmployees` for NetPayLines with `PaymentMethod=DirectDeposit`.
4. Verify `EmployeeBankAccounts` for `VerificationStatus=Verified` and `IsActive=true`.
5. If bank accounts are missing verification, instruct employee to re-submit bank verification or use an alternate payment method.
6. If other errors occur, review server logs and audit entries.

Escalation
- If approval fails due to DB errors or concurrency, escalate to engineering with logs and DB snapshot.

Security notes
- Do not request or view full bank account numbers. Use last-4 for customer confirmation.
