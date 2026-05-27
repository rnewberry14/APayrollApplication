# Tax Deposits Import Admin Notes

The Tax Deposits Import workflow stages, validates, previews, and saves local user-entered deposit records.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Route

- `/import/tax-deposits`

## Required Permissions

- Setup/import access
- Local database write access through the application
- Local read access to the selected import file

## Validation

The workflow validates:

- Required mapped fields
- Local company match
- Deposit date
- Tax period start and end
- Tax period end not before start
- Positive amount

## Reporting

Saved records appear on Tax Liability Report as `User-entered deposit record` entries. The report does not mark liabilities as paid or verified.

## Cautions

- No tax payment is submitted.
- No tax filing is submitted.
- No IRS or state system is called.
- Do not treat imported confirmation numbers as verification by ClearPath Payroll.
