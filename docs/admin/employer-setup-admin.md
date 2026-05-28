# Employer Setup Administration

ClearPath Payroll stores employer setup and payroll settings locally. The software organizes user-entered payroll data and does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Feature Scope

- Employer profile setup at `/setup/employer`.
- Employer payroll settings at `/setup/employer/payroll-settings`.
- Alias routes under `/employers`.
- Payroll item maintenance for earning, deduction, tax, reimbursement, and memo-only records.

## Administration Notes

- FEIN is stored from user entry after trim/basic formatting cleanup.
- SUIN means state unemployment insurance number entered by user.
- SEIN means state employer identification number entered by user.
- Phone numbers are stored as `(123) 456-7890` when 10 digits are present.
- Rate fields are percent values stored to 4 decimal places.
- SUTA state is stored as a two-letter abbreviation.
- Filing Frequency / Depositor Type is a user-selected value for reporting and reminder organization.

## Operational Cautions

- User-entered rate. Verify with official agency records.
- The screen does not determine rates.
- The screen does not submit ACH.
- The screen does not submit tax filings.
- Full bank account numbers and routing numbers are outside the intended field usage.
