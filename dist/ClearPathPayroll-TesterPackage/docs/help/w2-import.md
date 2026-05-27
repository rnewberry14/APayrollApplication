# W-2 Spreadsheet Import

W-2 Spreadsheet Import saves user-provided spreadsheet rows as local historical W-2 records marked `historical user-entered data`.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Supported Files

- CSV
- Tab-delimited text
- First worksheet of `.xlsx`

PDF/OCR import is not included in this version.

## Required Columns

- `TaxYear`
- `EmployerName`
- `EmployerEIN`
- `EmployerAddress`
- `EmployeeFirstName`
- `EmployeeLastName`
- `EmployeeSSNLast4`
- `EmployeeAddress`
- `Box1Wages`
- `Box2FederalTaxWithheld`
- `Box3SocialSecurityWages`
- `Box4SocialSecurityTaxWithheld`
- `Box5MedicareWages`
- `Box6MedicareTaxWithheld`

Optional columns: `Box12CodeAndAmount`, `Box14DescriptionAndAmount`, `StateWages`, `StateTaxWithheld`, `LocalWages`, `LocalTaxWithheld`.

## Steps

1. Open `/import/w2`.
2. Select a local W-2 spreadsheet file.
3. Review mapped columns.
4. Review preview rows.
5. Select **Validate W-2 Rows**.
6. Review validation errors.
7. Select **Save Historical W-2 Records** after validation passes.

## Cautions

- Do not import full SSNs.
- EmployeeSSNLast4 accepts last four digits only.
- Employer EIN is masked after import.
- The workflow does not create tax filings.
- The workflow does not validate W-2 correctness.
- Files are not transmitted externally.
