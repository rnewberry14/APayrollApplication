# Checks Import

Checks import creates Draft after-the-fact payroll runs from user-provided local files. The workflow supports `/import/checks` for existing employees and `/import/employees-and-checks` for files that may include employee identifiers that are not already in the local database.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Who Can Use It

Local payroll administrators and prototype testers can use this workflow to enter after-the-fact payroll data from CSV, tab-delimited text, or `.xlsx` files.

## Required Columns

- `CompanyIdentifier`
- `EmployeeIdentifier`
- `PayDate`
- `PayPeriodStart`
- `PayPeriodEnd`
- `GrossPay`
- `RegularHours`
- `OvertimeHours`
- `EmployeeFederalTax`
- `EmployeeStateTax`
- `EmployeeLocalTax`
- `SocialSecurityTax`
- `MedicareTax`
- `Deductions`
- `NetPay`
- `PaymentMethod`

Optional columns: `CheckNumber`, `DirectDepositLast4`.

## Steps

1. Open `/import/checks` or `/import/employees-and-checks`.
2. Select a local file.
3. Review mapped columns.
4. Review preview rows.
5. Select **Validate Checks**.
6. Review any mismatch or missing-record errors.
7. Select **Save Draft Payroll Runs** after validation passes.
8. Open Payroll Preview for the created Draft run before any later approval workflow.

## Cautions

- Imported checks are not auto-approved.
- The workflow does not submit ACH.
- The workflow does not file taxes.
- Imported data is user-provided and requires review.
- Do not include full SSNs, full bank account numbers, or routing numbers.
