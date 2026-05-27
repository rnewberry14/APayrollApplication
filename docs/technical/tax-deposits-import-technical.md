# Tax Deposits Import Technical Notes

The tax deposits import workflow uses the generic import framework and persists local `TaxDepositRecord` rows after confirmation.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Components

- `TaxDepositRecord`: local user-entered deposit record entity.
- `TaxDepositsImportService`: tax deposit validation and confirmation service.
- `TaxDepositsImport.razor`: `/import/tax-deposits` import page.
- `TaxLiabilityReportService`: includes user-entered deposits in report output.

## Flow

1. Parse local file through the generic parser system.
2. Stage rows in `ImportBatch` and `ImportRow`.
3. Validate tax deposit mappings and values.
4. Show preview and validation errors.
5. Save `TaxDepositRecord` rows only after confirmation.
6. Mark the import batch as imported.

## External Systems

The workflow does not submit payments, file taxes, call IRS systems, call state systems, upload files, or add telemetry.

## Reporting

Tax Liability Report displays imported deposits in a separate `User-entered deposit records` section. These records support review and reporting only and are not marked as verified by ClearPath Payroll.
