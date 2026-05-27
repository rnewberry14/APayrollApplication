# File Import Framework Troubleshooting

This guide covers common local import issues.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Common Errors

- **Supported import file types are CSV, tab-delimited text, and .xlsx workbooks.**
  The selected file extension is not supported.

- **Binary .xls import is not supported by the local parser.**
  Save the workbook as `.xlsx`, `.csv`, or tab-delimited text.

- **At least one column mapping is required.**
  Enter at least one target field before validation.

- **Required mapped field is missing a value.**
  A row has a blank value for a mapping marked required.

## Troubleshooting Steps

1. Confirm the file has a header row.
2. Confirm the file extension is `.csv`, `.txt`, `.tsv`, `.tab`, or `.xlsx`.
3. For Excel files, place the import data on the first worksheet.
4. Review column mappings for blank target fields.
5. Remove full SSNs, full bank account numbers, and routing numbers before import.
6. Validate again after editing mappings or source data.
7. Confirm import only after errors are resolved.

## Cautions

- Do not use import files as a tax filing or ACH submission path.
- Do not transmit import files outside the local workflow.
- Demo mode databases can contain real-looking records; review the warning before importing real payroll data.
