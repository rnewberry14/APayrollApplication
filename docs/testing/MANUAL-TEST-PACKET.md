# ClearPath Payroll Manual Test Packet

Version: ____________________

Test date: ____________________

Tester name: ____________________

Tester package only. Do not use for real payroll. Use demo data only.

## Table Of Contents

- [A. Tester Safety Rules](#a-tester-safety-rules)
- [B. Startup Tests](#b-startup-tests)
- [C. Demo Data Tests](#c-demo-data-tests)
- [D. Employer Setup Tests](#d-employer-setup-tests)
- [E. Employee Setup Tests](#e-employee-setup-tests)
- [F. User-Defined Fields Tests](#f-user-defined-fields-tests)
- [G. Payroll Run Tests](#g-payroll-run-tests)
- [H. Check Calculation Tests](#h-check-calculation-tests)
- [I. Payroll Preview Tests](#i-payroll-preview-tests)
- [J. Approval Tests](#j-approval-tests)
- [K. Fake Direct Deposit Tests](#k-fake-direct-deposit-tests)
- [L. Paycheck Printing Tests](#l-paycheck-printing-tests)
- [M. Pay Stub Tests](#m-pay-stub-tests)
- [N. Payroll Register Tests](#n-payroll-register-tests)
- [O. Tax Liability Report Tests](#o-tax-liability-report-tests)
- [P. Import Tests](#p-import-tests)
- [Q. Official Source Library Tests](#q-official-source-library-tests)
- [R. Help Article Tests](#r-help-article-tests)
- [S. Security/Privacy Tests](#s-securityprivacy-tests)
- [T. Tester Feedback Form](#t-tester-feedback-form)

---

## A. Tester Safety Rules

- [ ] Do not use real payroll.
- [ ] Do not use real SSNs.
- [ ] Do not use real bank account numbers.
- [ ] Do not use real EINs.
- [ ] Do not use real tax API keys.
- [ ] Do not submit real ACH.
- [ ] Do not submit real tax filings.
- [ ] Use demo data only.

Expected Result:

The tester understands this is a local demo/test application and uses only fake/demo/sandbox data.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## B. Startup Tests

- [ ] Unzip package.
- [ ] Read `README-TESTERS.md`.
- [ ] Start app.
- [ ] Confirm browser opens.
- [ ] Confirm Local Demo Mode banner appears.
- [ ] Confirm no real ACH or tax filing is enabled.

Expected Result:

The local app starts, opens in a browser, and clearly shows demo/local safety messaging.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## C. Demo Data Tests

- [ ] Open demo data page.
- [ ] Create demo data.
- [ ] Confirm demo company exists.
- [ ] Confirm demo employees exist.
- [ ] Confirm demo pay schedule exists.
- [ ] Confirm fake bank tokens are used.
- [ ] Clear demo data.
- [ ] Recreate demo data.

Expected Result:

Demo data can be created, cleared, and recreated without using real payroll, SSN, bank, or tax data.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## D. Employer Setup Tests

- [ ] Open employer setup.
- [ ] Add employer.
- [ ] Edit employer.
- [ ] Confirm FEIN remains the user-entered value after save.
- [ ] Add employer tax settings.
- [ ] Add SUTA rate.
- [ ] Add payroll items.
- [ ] Confirm no advice language appears.

Expected Result:

Employer setup saves demo employer details, masks sensitive ID values, and uses neutral wording.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## E. Employee Setup Tests

- [ ] Add hourly employee.
- [ ] Add salary employee.
- [ ] Add tipped employee placeholder.
- [ ] Enter demographic information.
- [ ] Enter pay rates.
- [ ] Enter W-4 information.
- [ ] Enter state tax information.
- [ ] Add up to 5 direct deposit accounts.
- [ ] Confirm full SSN is not requested.
- [ ] Confirm full bank account is not displayed.
- [ ] Confirm validation messages work.

Expected Result:

Employee setup supports demo employee entry without requesting or displaying full SSNs or full bank account numbers.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## F. User-Defined Fields Tests

- [ ] Create user-defined field.
- [ ] Activate optional field template.
- [ ] Add field value to employee or payroll.
- [ ] Confirm no scripts/formulas execute.
- [ ] Confirm field appears where expected.

Expected Result:

Custom fields can be configured for demo use, and formula/script execution is not available.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## G. Payroll Run Tests

- [ ] Create regular payroll.
- [ ] Create after-the-fact payroll.
- [ ] Create correction payroll placeholder.
- [ ] Select employees.
- [ ] Enter regular hours.
- [ ] Enter overtime placeholder.
- [ ] Enter manual adjustment.
- [ ] Save draft payroll run.

Expected Result:

Payroll runs can be created in draft status using demo employees and demo pay data.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## H. Check Calculation Tests

- [ ] Calculate one hourly check.
- [ ] Calculate one salary check.
- [ ] Calculate tipped employee placeholder.
- [ ] Add pre-tax deduction.
- [ ] Add post-tax deduction.
- [ ] Recalculate one check.
- [ ] Recalculate all checks.
- [ ] Confirm negative net pay is blocked.
- [ ] Confirm minimum wage warning appears when applicable.

Expected Result:

Check calculation displays demo payroll amounts, warnings, and blocking errors where appropriate.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## I. Payroll Preview Tests

- [ ] Open payroll preview.
- [ ] Review gross pay.
- [ ] Review employee taxes.
- [ ] Review employer taxes.
- [ ] Review deductions.
- [ ] Review net pay.
- [ ] Review total cash required.
- [ ] Confirm warnings display.
- [ ] Confirm blocking errors prevent approval.
- [ ] Confirm no full SSNs or bank numbers display.

Expected Result:

Payroll preview shows calculated demo totals and does not expose full SSNs or full bank account numbers.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## J. Approval Tests

- [ ] Approve valid payroll.
- [ ] Confirm approved status.
- [ ] Confirm approved payroll cannot be recalculated.
- [ ] Confirm audit entry exists if visible.
- [ ] Try approving invalid payroll and confirm error.

Expected Result:

Valid demo payroll can be approved, invalid payroll is blocked, and approved payroll is protected from casual recalculation.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## K. Fake Direct Deposit Tests

- [ ] Submit fake direct deposit.
- [ ] Confirm fake batch reference appears.
- [ ] Try duplicate submission and confirm blocked.
- [ ] Confirm no real ACH occurred.
- [ ] Confirm direct deposit account display only shows last 4.

Expected Result:

Only fake/sandbox direct deposit activity occurs, duplicate submission is blocked, and only account last 4 is shown.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## L. Paycheck Printing Tests

- [ ] Open non-direct-deposit employee paycheck.
- [ ] Confirm check is printable.
- [ ] Confirm no full SSN appears.
- [ ] Confirm demo watermark appears.
- [ ] Print to PDF.

Expected Result:

The paycheck page prints a demo/user-controlled document without full SSNs or bank account numbers.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## M. Pay Stub Tests

- [ ] Open pay stub.
- [ ] Confirm company name.
- [ ] Confirm employee name.
- [ ] Confirm last 4 SSN only.
- [ ] Confirm pay period.
- [ ] Confirm pay date.
- [ ] Confirm earnings.
- [ ] Confirm taxes.
- [ ] Confirm deductions.
- [ ] Confirm net pay.
- [ ] Confirm direct deposit last 4 only.
- [ ] Confirm YTD placeholders.
- [ ] Print to PDF.

Expected Result:

The pay stub displays demo payroll details, last 4 SSN only, direct deposit last 4 only, and printable output.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## N. Payroll Register Tests

- [ ] Open payroll register.
- [ ] Filter by company.
- [ ] Filter by date range.
- [ ] Review run totals.
- [ ] Review employee-level summary.
- [ ] Export CSV.
- [ ] Confirm CSV excludes full SSNs and bank numbers.

Expected Result:

Payroll register filtering and CSV export work with demo data and do not include full SSNs or bank account numbers.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## O. Tax Liability Report Tests

- [ ] Open tax liability report.
- [ ] Filter by company.
- [ ] Filter by date range.
- [ ] Confirm employee withholding is separate from employer taxes.
- [ ] Confirm due date placeholder.
- [ ] Confirm payment status placeholder.
- [ ] Export CSV.
- [ ] Confirm no tax payment was submitted.

Expected Result:

The tax liability report displays demo reporting data and does not submit tax payments or filings.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## P. Import Tests

- [ ] Import employees from CSV.
- [ ] Import employees from Excel.
- [ ] Import checks from CSV.
- [ ] Import checks from Excel.
- [ ] Import tax deposits.
- [ ] Import QuickBooks Desktop exported report.
- [ ] Import QuickBooks Online exported file.
- [ ] Import W-2 spreadsheet.
- [ ] Import W-2 PDF text-based sample.
- [ ] Confirm scanned PDF fallback message appears.
- [ ] Confirm preview-before-save workflow.
- [ ] Confirm no file is uploaded externally.

Expected Result:

Imports use local files, require review before save, and do not transmit files externally.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## Q. Official Source Library Tests

- [ ] Add official source document.
- [ ] Add verbatim excerpt.
- [ ] Confirm citation appears.
- [ ] Confirm source URL appears.
- [ ] Confirm currentness warning appears.
- [ ] Confirm no interpretation/advice appears.

Expected Result:

Official source entries are shown as source material with citation/currentness information and no advice wording.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## R. Help Article Tests

- [ ] Open help docs.
- [ ] Confirm user can find Employee Setup article.
- [ ] Confirm user can find Payroll Run article.
- [ ] Confirm user can find Payroll Preview article.
- [ ] Confirm user can find Pay Stub article.
- [ ] Confirm user can find Import article.
- [ ] Confirm help articles are understandable.

Expected Result:

Help articles are easy for a non-technical tester to find and understand.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## S. Security/Privacy Tests

- [ ] Search screens for full SSN.
- [ ] Search screens for full bank account.
- [ ] Confirm no cloud upload.
- [ ] Confirm no telemetry.
- [ ] Confirm no production ACH.
- [ ] Confirm no tax filing.
- [ ] Confirm no advice language.

Expected Result:

The demo does not expose full sensitive identifiers, upload data to cloud services, submit real provider transactions, or provide advice.

Tester Notes:

________________________________________________________________________________

________________________________________________________________________________

---

## T. Tester Feedback Form

Use one row per issue or test section.

| Tester name | Date | App version | Test section | Pass/fail | Issue found | Screenshot attached yes/no | Severity | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| | | | | | | | | |
| | | | | | | | | |
| | | | | | | | | |
| | | | | | | | | |
| | | | | | | | | |

Additional Notes:

________________________________________________________________________________

________________________________________________________________________________

________________________________________________________________________________
