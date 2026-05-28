# Import Employees Technical Notes

The employee import workflow is implemented by `EmployeesOnlyImport.razor` and `EmployeesOnlyImportService`.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Target Fields

The workflow supports `FullName`, `FirstName`, and `LastName`. `PayType` remains a required target field. Name validation is handled as a composite rule:

- `FirstName` and `LastName`, or
- `FullName`

## Full Name Parsing

The parser accepts:

- `John Smith`
- `John A Smith`
- `Smith, John`

Uncertain values are flagged with `FullNameNeedsReview` and are not silently imported.

## Alias Matching

`FullName` aliases include `Full Name`, `Employee Name`, `Name`, `Worker Name`, and `Employee`.

## Privacy

The employee import workflow rejects full SSN, routing number, and full bank account mappings. Import files are parsed locally and are not uploaded by this workflow.
