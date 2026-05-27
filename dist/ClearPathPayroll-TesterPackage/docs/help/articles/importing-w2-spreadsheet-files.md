# Importing W-2 Spreadsheet Files

## What this page does

W-2 spreadsheet import saves reviewed historical user-entered W-2 data from local CSV, tab-delimited, or Excel files.

## When to use it

Use it to test historical W-2 setup from a spreadsheet.

## Before you begin

Prepare a fake/demo file with W-2 fields. Use SSN last four only.

## Step-by-step instructions

1. Open `/import/w2`.
2. Select a local spreadsheet or text file.
3. Map W-2 columns.
4. Review validation messages.
5. Preview rows.
6. Confirm the import after review.
7. Verify saved records use masked EIN and SSN last four only.

## What to check

- Full SSNs are rejected.
- Employer EIN is masked.
- Records are marked as historical user-entered data.
- Save happens only after confirmation.

## Common messages or errors

- Invalid SSN last four: use four digits.
- Required W-2 field missing: map or correct the file.

## What this page does not do

It does not create tax filings or validate W-2 accuracy.

## Privacy/safety notes

Do not import real W-2 data into the demo.

## Related articles

- [Importing W-2 PDF Files](importing-w2-pdf-files.md)
- [Importing Employees](importing-employees.md)
