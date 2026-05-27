# W-2 PDF Import Admin Notes

W-2 PDF import is a local-only prototype workflow for historical user-provided W-2 data. It uses local PDF text extraction only and does not use cloud OCR, external APIs, telemetry, hosted storage, tax filing, or ACH submission.

## Required permissions

Access is intended for users allowed to import historical payroll setup data in the local prototype environment.

## Data storage

Parsed batches are stored in:

- `W2ImportBatches`
- `W2ImportRecords`
- `W2ImportErrors`

The original PDF is not stored by default. Full SSNs and full EINs are reduced to last-four/masked values before persistence.

## Review requirement

Records are saved only after the user confirms:

`I reviewed the extracted W-2 information and understand it is user-provided historical data.`

## Cautions

ClearPath Payroll does not verify, validate, or guarantee W-2 accuracy and does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice. Imported records are for user review and local historical setup only.
