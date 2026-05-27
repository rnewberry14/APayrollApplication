Payroll Approval

What it does
- Allows authorized users to approve a calculated payroll run. Approval locks the run, records the approver and timestamp, and creates an audit log entry.

Who can use it
- Users with payroll approval permissions (Payroll Manager, Administrator).

Step-by-step
1. Navigate to Payroll → Preview for the desired run.
2. Review validation warnings and errors.
3. Ensure all direct deposit employees have verified bank accounts.
4. Click "Approve" and confirm the action.

Required permissions
- Role-based access: `PayrollManager` or `Administrator` (or equivalent permission to approve payroll).

Common errors
- "Payroll run X must be in Calculated status to approve": Run must be calculated first.
- "Payroll run X has no employees": Run is empty.
- "Direct deposit employees missing verified bank accounts": One or more direct deposit employees lack a verified, active bank account.

Troubleshooting steps
- If run is not calculated: Run payroll calculations or re-run calculation.
- If missing bank verification: Ask employees to add/verify bank account or switch payment method.
- If approval fails unexpectedly: Check server logs and audit entries for more details.

Payroll/Tax/ACH/Security cautions
- Approval is an auditable action; it should only be performed by authorized personnel.
- Do NOT display full bank account or SSN data in approval screens.
- Approved runs are locked against recalculation unless voided by an admin process.
- Ensure ACH batches are reviewed separately before submission to the bank.
