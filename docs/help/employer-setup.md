# Employer Setup and Payroll Settings

ClearPath Payroll provides local software tools for entering employer payroll setup data and payroll items. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## What It Does

- Stores employer contact and address details in the local database.
- Stores user-entered FEIN, SUIN, and SEIN values without replacing them with demo placeholders.
- Formats employer phone numbers as `(123) 456-7890` when 10 digits are entered.
- Stores user-entered employer rate fields with 4 decimal places.
- Stores SUTA state as a two-letter abbreviation selected from a state list.
- Stores Filing Frequency / Depositor Type as a user-selected value.
- Does not call tax APIs, submit ACH, or submit tax filings.

## Steps

1. Open **Payer/Employer > New/Add** or **Payer/Employer > Edit**.
2. Enter employer name, FEIN, contact, phone, address, and active status.
3. Select **Save**.
4. Open **Payer/Employer > Payroll Settings**.
5. Select the employer.
6. Enter SUIN and SEIN if used by the tester scenario.
7. Select the SUTA state.
8. Enter FUTA, SUTA, and local employer tax rates as percent values.
9. Select Filing Frequency / Depositor Type.
10. Add optional notes.
11. Select **Save Payroll Settings**.

## Common Errors

- Phone number has digits but not exactly 10 digits.
- Rate field contains non-numeric text.
- Rate is outside 0 through 100.
- Required employer fields are blank.
- Plain bank account numbers are entered into placeholder account fields.

## Cautions

User-entered rate. Verify with official agency records. Do not enter real payroll data in the local demo package.
