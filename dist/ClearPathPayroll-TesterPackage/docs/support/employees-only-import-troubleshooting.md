# Employees Only Import Troubleshooting

Use this page when an employee import file does not validate or confirm.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Common Errors

- **Required employee field is not mapped.**
  Map `FirstName`, `LastName`, and `PayType`.

- **Map either SSNLast4 or EmployeeNumber before importing employees.**
  The file needs one employee identity field.

- **SSNLast4 must contain exactly four digits.**
  Replace full SSNs with last-four values only.

- **DirectDepositLast4 must contain exactly four digits.**
  Replace full account values with last-four values only.

- **EmployeeNumber already exists for the selected company.**
  Edit the employee number or remove the duplicate row.

- **Hourly employees require an hourly rate greater than zero.**
  Add `HourlyRate` for rows with `PayType` of `Hourly`.

- **Salary employees require an annual salary greater than zero.**
  Add `AnnualSalary` for rows with `PayType` of `Salary`.

## Troubleshooting Steps

1. Confirm the file has a header row.
2. Confirm the selected company is correct.
3. Confirm the file extension is supported.
4. Review mappings for required fields.
5. Remove full SSNs, routing numbers, and full bank account numbers.
6. Check duplicate employee numbers.
7. Check duplicate name plus SSN last four combinations.
8. Validate again.

## Cautions

- Employee import is a local data workflow only.
- The workflow does not submit ACH.
- The workflow does not file taxes.
- The workflow does not provide payroll compliance advice.
