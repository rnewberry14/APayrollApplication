# Employer Setup Troubleshooting

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. Support content is limited to software operation.

## Common Errors

- **FEIN changed format**: The app may trim spaces and format 9 digits as `12-3456789`. It does not replace the value with `00-0000000`.
- **Phone number error**: Enter 10 digits. Spaces, dashes, dots, and parentheses are accepted.
- **Rate error**: Enter a number from 0 through 100. The app stores 4 decimal places.
- **SUTA state missing**: Select a state from the dropdown.
- **Filing Frequency / Depositor Type missing**: Select an option or use `Other / User Defined`.
- **Account placeholder rejected**: Enter a token or masked reference instead of a plain bank account number.

## Troubleshooting Steps

1. Confirm the selected employer exists.
2. Review validation messages at the top of the form.
3. Re-enter phone numbers with 10 digits.
4. Re-enter rates as percent values.
5. Confirm SUTA state and Filing Frequency / Depositor Type dropdown values.
6. Retry the save action.

## Safety Cautions

User-entered rate. Verify with official agency records. The setup pages do not submit ACH, submit tax filings, call production providers, or certify rate correctness.
