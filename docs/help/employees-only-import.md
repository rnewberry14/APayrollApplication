# Employees Only Import

The Employees Only Import page imports employee setup records from CSV, tab-delimited text, or `.xlsx` files into the local ClearPath Payroll database after review and confirmation.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Who Can Use It

This workflow is for local payroll administrators and prototype testers who need to create employee setup records from a local file.

## Required Columns

- `FirstName` and `LastName`, or `FullName`
- `PayType`
- Either `SSNLast4` or `EmployeeNumber`

Hourly employees also need `HourlyRate`. Salary employees also need `AnnualSalary`.

## Optional Columns

- `MiddleInitial`
- `FullName`
- `Address1`
- `Address2`
- `City`
- `State`
- `ZipCode`
- `Email`
- `Phone`
- `HireDate`
- `EmploymentStatus`
- `HourlyRate`
- `AnnualSalary`
- `FederalFilingStatus`
- `ExtraWithholding`
- `StateTaxState`
- `StateFilingStatus`
- `DirectDepositLast4`

## Steps

1. Open `/import/employees`.
2. Select a company.
3. Select a local CSV, tab-delimited, or `.xlsx` employee file.
4. Review the mapping screen.
5. Map source columns to employee fields.
6. Review the preview.
7. Select **Validate Employees**.
8. Review validation errors.
9. Select **Confirm Employee Import** after validation passes.

## Cautions

- Do not import full SSNs.
- Do not import full bank account numbers.
- Do not import routing numbers.
- Use `SSNLast4` and `DirectDepositLast4` only.
- This workflow does not submit ACH, file taxes, call tax APIs, or upload files to cloud storage.
- Missing optional address and birth date values use local placeholders that can be edited after import.
