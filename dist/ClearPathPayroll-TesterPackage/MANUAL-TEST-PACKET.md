# ClearPath Payroll Manual Test Packet

Tester package only. Do not use for real payroll. Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.

## Suggested Local Demo Test Path

1. Start the app from the extracted tester package.
2. Open `/prototype-test-checklist`.
3. Confirm local prototype mode messaging is visible.
4. Open `/demo/seed-data`.
5. Create demo data.
6. Review employer setup.
7. Review employee setup.
8. Review pay schedule setup.
9. Create a draft payroll run.
10. Calculate payroll.
11. Preview payroll.
12. Approve payroll after reviewing demo values.
13. Submit fake/sandbox direct deposit only when available.
14. Open a pay stub.
15. Open payroll register.
16. Open tax liability report.
17. Confirm no real ACH was submitted.
18. Confirm no real tax filing was submitted.
19. Confirm no full SSN or full bank account number is displayed.

## Import Tests

- Use fake/demo CSV, Excel, and PDF files only.
- Confirm W-2 spreadsheet import requires review before saving.
- Confirm W-2 PDF import does not store the original PDF by default.
- Confirm scanned/image-only PDFs show the fallback message.
