# User-Defined Payroll Fields Troubleshooting

ClearPath Payroll does not provide legal, tax, financial, accounting, or payroll compliance advice. This document covers software behavior only.

## Common Errors

- **Field will not save**: Confirm field name and field code are present.
- **Invalid field code**: Use letters, numbers, underscores, or hyphens.
- **List options error**: Add at least one option for list fields.
- **Script text blocked**: Remove script tags, JavaScript URL text, or template-style expressions.
- **Duplicate field code**: Use a field code that is unique for the selected company.
- **Value rejected later**: Confirm the field value matches the configured data type.

## Troubleshooting Steps

1. Confirm a company is selected.
2. Review validation messages on the page.
3. Replace unsafe text with plain text.
4. For list fields, enter comma-separated options or one option per line.
5. Save again.

## Security Cautions

Do not enter full SSNs, full bank account numbers, routing numbers, API keys, or secrets in optional custom fields.

## Payroll, Tax, And ACH Cautions

The feature does not submit ACH, submit tax filings, call production APIs, or provide guidance about required payroll fields.
