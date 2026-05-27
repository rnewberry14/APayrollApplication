# Importing W-2 PDF Files

## What this page does

W-2 PDF import extracts likely W-2 fields from local text-based PDFs for tester review and correction.

## When to use it

Use it when testing local PDF text extraction for historical user-provided W-2 data.

## Before you begin

Use a fake/demo text-based PDF. Do not use scanned real W-2 files.

## Step-by-step instructions

1. Open `/import/w2-pdf`.
2. Select a local W-2 PDF.
3. Click `Parse PDF`.
4. Review extracted fields.
5. Correct missing or low-confidence fields.
6. Confirm reviewed data.
7. Click `Save Reviewed Records`.
8. Test a scanned/image-only PDF and confirm fallback message appears.

## What to check

- Full SSNs and EINs are not displayed after parsing.
- Raw preview is limited and masked.
- Original PDF is not stored by default.
- Save requires review confirmation.

## Common messages or errors

- No selectable text: use manual entry or spreadsheet import.
- Low confidence: review and correct fields.

## What this page does not do

It does not use cloud OCR, upload PDFs, create filings, or verify W-2 accuracy.

## Privacy/safety notes

Do not upload or import real W-2 PDFs into the demo.

## Related articles

- [Importing W-2 Spreadsheet Files](importing-w2-spreadsheet-files.md)
- [How to Report Feedback](how-to-report-feedback.md)
