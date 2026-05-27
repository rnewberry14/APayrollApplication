# File Import Framework Technical Notes

The import framework adds local staging entities and parser services for review-and-confirm imports.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Models

- `ImportBatch`: import type, file format, status, counts, confirmation timestamps, and warning acknowledgement.
- `ImportRow`: local row snapshot as JSON plus row validation/import status.
- `ImportError`: batch-level or row-level validation error without sensitive row data in the message.
- `ImportMapping`: source column to target field mapping.

## Services

- `IImportFileParser`: parser abstraction.
- `CsvImportFileParser`: local CSV parser with quoted value handling.
- `TabDelimitedImportFileParser`: local tab-delimited parser.
- `ExcelImportFileParser`: local `.xlsx` first-worksheet parser using built-in ZIP/XML APIs.
- `ImportService`: selects parsers, stages batches, validates mappings, records errors, and confirms batches.

## Data Flow

1. The user selects a local file on `/setup/imports`.
2. The selected parser reads the stream locally.
3. The page displays detected columns and preview rows.
4. The user maps columns and marks required fields.
5. `ImportService.CreateStagedBatchAsync` stores the staged batch locally and validates rows.
6. `ImportService.ConfirmImportAsync` marks valid rows and the batch as imported.

The framework does not upload files, call external services, submit ACH, submit tax filings, or store original files permanently.

## Security Notes

- Do not log row values.
- Use token or placeholder fields for bank data.
- Use SSN last four only.
- Keep imported data in the configured local database.
- Treat tax deposit imports as local data records only.
