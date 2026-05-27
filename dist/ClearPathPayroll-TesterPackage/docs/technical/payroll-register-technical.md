Payroll Register Technical Notes

What it does
- Provides a payroll register report from stored payroll run data.
- Filters by company and payroll run pay date range.
- Exports report data to CSV for auditing.

Implementation details
- `PayrollRegisterService` queries `PayrollRuns` with company and employee data.
- Employee summary rows are built from `PayrollRunEmployees` and their `Employee` navigation property.
- Totals are aggregated from payroll run totals only, avoiding raw SSN and bank data exposure.
- CSV export is generated from the report model with safe CSV escaping.

Data flows
- User selects filters in `PayrollRegister.razor`.
- The page calls `PayrollRegisterService.GetPayrollRegisterAsync(...)`.
- The report model contains runs, employee summaries, and roll-up totals.
- The page builds a base64 CSV data URI and exposes a download link.

Security
- Does not include full SSNs or full bank account numbers.
- Only company names, payroll run IDs, pay dates, employee names, and monetary totals are returned.
- Uses `AsNoTracking()` for read-only report queries.
