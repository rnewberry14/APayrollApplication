# W-2 PDF Import Technical Notes

## Components

- Route: `/import/w2-pdf`
- Page: `W2PdfImport.razor`
- Service: `W2PdfImportService`
- Parser: `W2PdfTextParser`
- Models: `W2ImportBatch`, `W2ImportRecord`, `W2ImportError`
- Tables: `W2ImportBatches`, `W2ImportRecords`, `W2ImportErrors`

## Extraction

The workflow uses the local `UglyToad.PdfPig` package to read selectable PDF text on the user's machine. It does not use cloud OCR, external APIs, telemetry, hosted storage, tax filing, or ACH submission.

The parser uses conservative regular expressions for common W-2 labels such as:

- `Wages, tips, other compensation`
- `Federal income tax withheld`
- `Social security wages`
- `Medicare wages and tips`
- `State wages, tips, etc.`

When text is missing or labels are not recognized, the record is marked for manual review and parser messages are shown.

## Persistence

Original PDF files are not stored by default. `RawExtractedTextPreview` is limited and sanitized. Full SSNs and full EINs are masked before display or save. Records require `UserConfirmed = true` before `SaveReviewedImportAsync` persists the batch.

## Limitations

PDF layouts vary. This parser extracts likely values from text-based PDFs only and does not validate W-2 correctness.
