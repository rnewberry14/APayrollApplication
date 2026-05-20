Tax Liability Report Technical Notes

What it does
- The Tax Liability Report loads payroll runs and groups tax amounts by tax type.
- It separates employee withholding (`TaxLines`) from employer tax liabilities (`EmployerTaxLines`).
- It also includes placeholder strings for due date and payment status, to support review and compliance tracking.

Implementation details
- `TaxLiabilityReportService` queries `PayrollRuns` with related `Company`, `PayrollRunEmployees.TaxLines`, and `EmployerTaxLines`.
- Employee withholding groups are built by grouping `TaxLines` by `TaxType`.
- Employer taxes are built by grouping `EmployerTaxLines` by `TaxType`.
- The report calculates run-level totals and aggregate totals for all selected payroll runs.
- CSV export is implemented with `TaxLiabilityReportService.BuildTaxLiabilityCsv`.

Razor page
- `Components/Pages/TaxLiabilityReport.razor` provides filters, table display, and CSV download support.
- The page is routed at `/reports/tax-liability`.

Permissions
- Requires read access to payroll run and tax line data.

Payroll/tax/security cautions
- The report is not used to submit tax payments.
- It does not expose sensitive bank or employee SSN data.
- Because due date and payment status are placeholders, downstream payment workflows must use a separate payment service.
