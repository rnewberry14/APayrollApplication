# Employer Setup Technical Notes

ClearPath Payroll provides local software tools for payroll data entry and storage. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Routes

- `/setup/employer`
- `/setup/employer/payroll-settings`
- `/employers/select`
- `/employers/new`
- `/employers/edit`
- `/employers/payroll-settings`

## Domain Model

`Company` stores employer contact, address, FEIN, SUIN, SEIN, SUTA state, employer rate fields, Filing Frequency / Depositor Type, and employer tax notes.

Rate fields are stored as decimal percent values with 4 decimal places:

- `FutaRatePlaceholder`
- `SutaRate`
- `LocalEmployerTaxRate`

## Helpers

- `PhoneNumberFormatter` accepts common phone punctuation and formats 10 digits as `(123) 456-7890`.
- `RateFormatter` parses percent rate text and formats rates as `12.3456`.
- `USStateList` provides U.S. states plus DC.
- `FilingFrequencyOptions` provides the approved Filing Frequency / Depositor Type options.

## Persistence

EF Core maps employer setup fields to `Companies`. The migration `20260528100000_AddEmployerFieldFormatting` adds `SUIN`, `SEIN`, and `LocalEmployerTaxRate`.

## External Calls

No tax API, ACH API, telemetry, cloud storage, or hosted database dependency is added by this feature.

## Safety Cautions

User-entered rate. Verify with official agency records. Full bank account numbers and routing numbers are not expected in placeholder fields.
