# Tax Deposits Import

Tax Deposits Import saves historical tax deposit entries as local “User-entered deposit record” records for review and reporting.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Who Can Use It

Local payroll administrators and prototype testers can import historical deposit records from CSV, tab-delimited text, or `.xlsx` files.

## Columns

- `CompanyIdentifier`
- `DepositDate`
- `TaxPeriodStart`
- `TaxPeriodEnd`
- `TaxType`
- `Agency`
- `Amount`
- `ConfirmationNumber`
- `PaymentMethod`
- `Notes`

## Steps

1. Open `/import/tax-deposits`.
2. Select a local tax deposit file.
3. Review mapped columns.
4. Review preview rows.
5. Select **Validate Tax Deposits**.
6. Review validation errors.
7. Select **Save User-Entered Deposit Records** after validation passes.
8. Open Tax Liability Report to view user-entered deposit records.

## Cautions

- This workflow does not submit payments.
- This workflow does not file taxes.
- This workflow does not call IRS or state systems.
- Imported deposit records are not claimed to be verified.
- Imported records are user-entered historical records for local review and reporting.
