# W-2 PDF Import Troubleshooting

This feature extracts selectable text from local PDF files. It does not OCR scanned images and does not upload PDFs to external services.

## Common issues

| Issue | Likely cause | Local troubleshooting |
| --- | --- | --- |
| No text extracted | PDF is scanned or image-only | Use manual entry or the spreadsheet W-2 import workflow |
| Missing fields | W-2 layout differs from parser aliases | Enter or correct values in the review grid |
| Low confidence | Required name, employer, tax year, SSN last 4, or amount fields were not found | Review parser messages and edit the record |
| Save is disabled | Review confirmation is not checked | Review the record and check the confirmation box |
| Save fails | Required fields or at least one amount are missing | Correct highlighted values and save again |

## Support boundaries

Support does not ask for full SSNs, full EINs, full bank account numbers, passwords, or API keys.

ClearPath Payroll does not verify, validate, or guarantee W-2 accuracy and does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
