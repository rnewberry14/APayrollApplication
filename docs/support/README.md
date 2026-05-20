# Support Guidance for ClearPath Payroll

## Purpose
This document explains support-safe handling for the payroll tax and direct deposit features.

## What support should never ask a customer for
- Full bank account numbers.
- Full routing numbers.
- SSNs or any personally identifiable information beyond safe, masked values.
- API keys, ACH processor credentials, or payroll vendor secrets.
- Passwords or two-factor authentication codes.

## Support-safe troubleshooting steps

### Direct Deposit and ACH Batches
1. Ask for the batch reference or payroll run ID, not raw bank data.
2. Confirm the employee has an active `EmployeeBankAccount` with tokenized account references.
3. Verify the direct deposit batch status and totals.
4. Inspect `ReturnCode` and `ReturnDescription` for rejected or returned items.
5. Confirm the customer is using the correct pay period, pay date, and payroll run.

### ACH Return Code Troubleshooting
When a customer reports rejected or returned payments, ask for the return code and use the table below to guide resolution:
- **R01 (Invalid account number)**: Verify with customer that account number is correct; may indicate typo or account closure.
- **R02 (Account closed)**: Confirm with customer that the account is still active; request new account information.
- **R03 (No account/unable to locate)**: Account may not exist at the receiving bank; verify all details with customer.
- **R04 (Invalid routing number)**: Verify routing number is correct for the receiving bank; most common for smaller credit unions.
- **R05 (Debit revoked)**: Customer has revoked authorization; customer must re-authorize if they wish to receive direct deposits.
- **R06 (Returned per customer request)**: Customer requested return; ask if they still want direct deposits.
- **R07 (Settlement error)**: System or bank processing error; may resolve on retry.
- **R08 (Routing check digit error)**: Invalid routing number format; verify with customer and ABA lookup.
- **R09 (Incorrect individual ID)**: May indicate mismatch between name and account; verify customer identity.
- **R10 (Unable to process)**: Generic error; ask for details and escalate to engineering if issue persists.

## Security warnings
- The system is designed to avoid storing raw account data.
- Only token placeholders and the last 4 digits of accounts are permitted.
- If a customer offers raw routing or account numbers, tell them not to share that information through support channels.

## When to escalate to engineering
- If a direct deposit batch is in `Submitted` but not progressing to `Settled`.
- If a tax calculation result appears inconsistent with payroll inputs.
- If the support team sees any evidence of raw bank account or SSN data in logs.
- If the customer reports system errors not explained by return codes or invalid batch totals.
