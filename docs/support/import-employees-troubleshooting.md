# Import Employees Troubleshooting

Use this guide when an employee import file does not validate.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice.

## Common Messages

- **Map either FirstName and LastName, or map FullName before importing employees.**
  The mapping needs either separate first and last name columns or one full name column.

- **FullName could not be confidently parsed.**
  Edit the source file to use a common full-name format or map separate `FirstName` and `LastName` columns.

- **EmployeeNumber or SSNLast4 is required.**
  Map one employee identity field.

- **SSNLast4 must contain exactly four digits.**
  Replace full SSNs with last-four values only.

## Troubleshooting Steps

1. Confirm the file has a header row.
2. Open the mapping panel.
3. Map either `FirstName` and `LastName`, or map `FullName`.
4. Confirm `PayType` is mapped.
5. Confirm either `EmployeeNumber` or `SSNLast4` is mapped.
6. Validate again.

## Safety Notes

Support should not ask testers to send full SSNs, full bank account numbers, routing numbers, passwords, API keys, or live payroll data.
