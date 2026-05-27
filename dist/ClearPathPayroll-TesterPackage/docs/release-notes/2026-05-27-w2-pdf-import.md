# 2026-05-27 W-2 PDF Import

Added a local prototype W-2 PDF import workflow at `/import/w2-pdf`.

## Added

- Local PDF text extraction for text-based W-2 PDFs.
- Review grid with editable extracted W-2 fields.
- Confirmation-required save workflow for historical user-provided data.
- W-2 PDF import batch, record, and error models.
- Parser warnings for missing or low-confidence fields.
- Masking so full SSNs and full EINs are not stored or displayed after parsing.
- Documentation and unit tests.

## Notes

This feature does not OCR scanned PDFs, upload files, create tax filings, submit ACH, or verify W-2 accuracy. ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
