# File Import Framework

ClearPath Payroll provides a local file import workflow for CSV, tab-delimited text, and `.xlsx` files. The workflow parses a selected local file, detects columns, allows column mapping, validates rows, shows a preview, and saves the staged import batch after confirmation.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Who Can Use It

The import workflow is intended for local prototype testers and payroll administrators who maintain company, employee, check, and tax deposit data in the local ClearPath Payroll database.

## Supported Import Types

- Entire company import
- Employers only
- Employees only
- Employees and checks only
- Checks only
- Tax deposits only

## Step-By-Step

1. Open **Setup > File Import**.
2. Select the import type.
3. Select a local `.csv`, `.txt`, `.tsv`, `.tab`, or `.xlsx` file.
4. Review the detected columns and preview rows.
5. Map source columns to target fields.
6. Mark required mappings when blank values need to block confirmation.
7. Select **Validate Rows**.
8. Review validation errors and preview data.
9. Select **Confirm Import** when the staged data is acceptable.

## Permissions

Use an account or local workstation profile with access to setup/import features and the local database.

## Cautions

- Files are parsed locally and are not uploaded to cloud storage by this workflow.
- Original files are not stored permanently by this workflow.
- Do not include full SSNs or full bank account numbers in import files.
- Tax deposits imported through this framework are data records only. This workflow does not submit tax filings or payments.
- ACH/direct deposit data imported through this framework is data only. This workflow does not submit ACH.
- In demo mode, review the warning before importing real payroll data into a demo database.
