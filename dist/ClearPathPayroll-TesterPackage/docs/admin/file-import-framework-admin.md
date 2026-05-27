# File Import Framework Admin Notes

The file import framework stages local file data for review and confirmation. It stores import batches, rows, mappings, and validation errors in the local database.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Who Can Use It

Administrators responsible for local payroll setup and data conversion can use the import page. Access to setup screens and the local database is expected.

## Required Permissions

- Setup page access
- Local database write access through the application
- Read access to the selected local import file

## Administration Steps

1. Prepare a CSV, tab-delimited, or `.xlsx` file with header columns.
2. Open `/setup/imports`.
3. Pick the import type.
4. Select the local file.
5. Review detected columns.
6. Map columns to target fields.
7. Mark required fields for validation.
8. Validate rows.
9. Review errors.
10. Confirm the import batch.

## Operational Notes

- The first worksheet of `.xlsx` files is parsed.
- Binary `.xls` files are not parsed by the local parser.
- Confirmed batches are marked imported in the local database.
- Row data is not written to application logs.
- Original files are not retained unless a later feature explicitly adds that option.

## Payroll, Tax, ACH, And Security Cautions

- This framework does not submit ACH.
- This framework does not submit tax filings.
- This framework does not call external tax APIs.
- Imported account identifiers need token or placeholder values, not full account numbers.
- Imported employee identifiers need last-four SSN values only.
- Review applicable official sources for filing, payment, and recordkeeping requirements.
