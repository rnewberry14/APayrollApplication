Tax Liability Report Admin Guide

What it does
- Provides administrators with a payroll run report that separates employee withholding and employer tax liabilities.
- Displays payroll run pay date, due date placeholder, payment status placeholder, and grouped tax type totals.

Who can use it
- Payroll administrators and finance managers with reporting access.

How to use it
1. Navigate to the Tax Liability Report from the main menu.
2. Optionally select a specific company.
3. Choose the pay date range for the payroll runs you want to review.
4. Click Refresh.
5. Review the tax liability groups under each payroll run.
6. Export the report to CSV for external reconciliation.

Required permissions
- Administrator read access to payroll and tax reporting data.

Common errors
- No payroll runs visible: company may not have payroll data for the selected range.
- Tax groups missing: payroll run may not have generated employee or employer tax lines yet.

Troubleshooting
- Confirm the company is active and has at least one approved or completed payroll run.
- Confirm tax calculations were completed for the payroll run.
- If the report still shows no data, check that the payroll run pay date falls inside the selected range.

Payroll/tax/security cautions
- This report is for review only and does not submit taxes.
- Taxes should be verified against the external tax API response before payment.
- Do not expose full SSNs, bank account numbers, or API secrets in this report.
