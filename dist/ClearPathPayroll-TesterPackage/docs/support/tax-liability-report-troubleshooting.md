Tax Liability Report Troubleshooting

What it does
- Helps support staff identify why the tax liability report returns no data or incorrect totals.

Common problems
- No payroll runs appear.
- Employee withholding groups are missing.
- Employer tax groups are missing.
- Totals do not match payroll run expectations.

Troubleshooting steps
1. Confirm the selected company is active.
2. Confirm the pay date range includes the payroll run pay date.
3. Confirm the payroll run contains employee tax lines (`TaxLines`) and employer tax lines (`EmployerTaxLines`).
4. Confirm the payroll run was calculated and not deleted.
5. If totals differ from expected liability amounts, verify the payroll run totals and tax line amounts in the database.

Support notes
- This report uses placeholder due date and payment status values for compliance review.
- It is not a tax payment submission report.
- If tax values are wrong, the underlying payroll calculation or tax calculation service likely needs review.
