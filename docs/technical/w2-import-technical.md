# W-2 Import Technical Notes

The W-2 spreadsheet import workflow uses the generic import framework and persists local `W2HistoricalRecord` rows after confirmation.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Components

- `W2HistoricalRecord`
- `W2ImportService`
- `W2Import.razor`
- `ImportService`
- Existing CSV, tab-delimited, and `.xlsx` parsers

## Flow

1. Parse local spreadsheet file.
2. Stage rows in `ImportBatch` and `ImportRow`.
3. Validate W-2 mappings and values.
4. Show preview and validation errors.
5. Save `W2HistoricalRecord` rows only after confirmation.
6. Mark the import batch as imported.

## Security

- Full employee SSNs are rejected.
- Employer EIN values are masked before storage.
- Records are marked `historical user-entered data`.
- No external transmission, OCR, or filing creation is added.
