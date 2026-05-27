# W-2 PDF Import

The W-2 PDF import page lets a local prototype tester select a text-based W-2 PDF, extract likely W-2 fields locally, review and correct the values, and save reviewed records as historical user-provided data.

ClearPath Payroll does not verify, validate, or guarantee the accuracy of imported W-2 data. It does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.

## Who can use it

Users with access to local prototype import tools can use this page. Imported records remain in the local database.

## Steps

1. Open `/import/w2-pdf`.
2. Select a local text-based W-2 PDF.
3. Click `Parse PDF`.
4. Review the extracted values and parser messages.
5. Correct missing or low-confidence fields.
6. Check the confirmation box after review.
7. Click `Save Reviewed Records`.

Scanned or image-only PDFs may not contain selectable text. Use manual entry or the spreadsheet W-2 import workflow when PDF text cannot be extracted.

## Cautions

Original PDF files are not stored by default. Full SSNs and full EINs are not stored or displayed after parsing. The page does not create payroll runs, submit tax filings, submit ACH, or transmit files externally.
