# Employer Setup Troubleshooting

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. Support content is limited to software operation.

## Common Errors

- **Employer will not save**: Confirm required fields have values and the email format is valid.
- **FEIN display looks masked**: Expected behavior after save.
- **Account placeholder rejected**: Enter a token or masked reference instead of a plain account number.
- **Payroll item will not save**: Confirm item code and item name are present.
- **End date error**: Confirm the end date is not earlier than the effective date.
- **Duplicate item code**: Use a unique item code for the selected company.

## Troubleshooting Steps

1. Confirm Local Demo Mode or the private deployment database is available.
2. Confirm the selected company exists and is active for payroll settings.
3. Review validation messages at the top of the form.
4. Replace plain account numbers with placeholder or token references.
5. Retry the save action.

## Payroll, Tax, ACH, And Security Cautions

Verify with official agency records. The setup pages do not submit ACH, submit tax filings, call production providers, or certify rate correctness. Avoid entering full bank account numbers or routing numbers.
