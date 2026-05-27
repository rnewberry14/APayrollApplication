# Employer Setup Administration

ClearPath Payroll stores employer setup and payroll settings locally. The software organizes user-entered payroll data and does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Feature Scope

- Employer profile setup at `/setup/employer`.
- Employer payroll settings at `/setup/employer/payroll-settings`.
- Payroll item maintenance for earning, deduction, tax, reimbursement, and memo-only records.
- Local database storage only.

## Required Permissions

Administrative access to the local ClearPath Payroll instance and the local database.

## Setup Steps

1. Create or edit an employer on **Setup > Employer**.
2. Enter FEIN and confirm the saved display is masked.
3. Open **Setup > Payroll Settings**.
4. Select the employer.
5. Enter user-entered rate values and placeholder account references.
6. Add payroll items that match the employer's internal payroll setup.

## Operational Cautions

- Verify with official agency records.
- Placeholder fields are not tax API lookup results.
- This screen does not determine rates or file returns.
- This screen does not submit ACH.
- Full bank account numbers and routing numbers are outside the intended field usage.

## Security Notes

FEIN and account reference displays are masked. Limit local database access to authorized users.
