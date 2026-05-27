# Final Tester Package Checklist

Package date: ____________________

Package version/build: ____________________

Reviewer: ____________________

Tester package only. Do not use for real payroll. Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.

## A. Build Checks

- [ ] `dotnet restore` completed.
- [ ] `dotnet build` completed.
- [ ] `dotnet test` completed.
- [ ] `dotnet publish` completed.
- [ ] Tester package zip created.

Notes:

________________________________________________________________________________

## B. Safety Checks

- [ ] No real API keys included.
- [ ] No user secrets included.
- [ ] No real SSNs included.
- [ ] No real bank account numbers included.
- [ ] No real EINs included.
- [ ] No production ACH enabled.
- [ ] No tax filing enabled.
- [ ] No telemetry enabled.
- [ ] No cloud database configured by default.

Notes:

________________________________________________________________________________

## C. Documentation Checks

- [ ] `README-TESTERS.md` included.
- [ ] Manual test packet included.
- [ ] Feedback form included.
- [ ] Startup troubleshooting included.
- [ ] Help articles included.
- [ ] Safety rules included.

Notes:

________________________________________________________________________________

## D. App Function Checks

- [ ] App starts locally.
- [ ] Local Demo Mode banner appears.
- [ ] Demo data can be created.
- [ ] Demo data can be reset.
- [ ] Payroll run can be created.
- [ ] Payroll can be calculated.
- [ ] Payroll can be approved.
- [ ] Fake direct deposit can be submitted.
- [ ] Pay stub opens.
- [ ] Payroll register opens.
- [ ] Tax liability report opens.
- [ ] CSV export works.

Notes:

________________________________________________________________________________

## E. Packaging Checks

- [ ] Zip file exists.
- [ ] Zip opens successfully.
- [ ] Start script exists.
- [ ] Reset script exists.
- [ ] Docs folder exists.
- [ ] Dist folder excludes source secrets.
- [ ] Dist folder excludes local real databases.

Notes:

________________________________________________________________________________

## F. Tester Instructions

### How to unzip

1. Save the tester package zip to the Windows computer.
2. Right-click the zip file.
3. Select `Extract All`.
4. Open the extracted folder.

### How to start

1. Open `README-TESTERS.md`.
2. Double-click `START-ClearPathPayroll.bat`.
3. If the browser does not open, go to `http://localhost:5080`.

### How to test

1. Open `/help`.
2. Open the Manual Test Packet.
3. Open `/demo/seed-data`.
4. Create demo data.
5. Follow the manual testing checklist.

### How to report feedback

1. Use `docs/testing/TESTER-FEEDBACK-FORM.md` for general notes.
2. Use `docs/testing/BUG-REPORT-TEMPLATE.md` for a specific problem.
3. Use `docs/testing/TESTER-ISSUE-LOG.md` to track issues.
4. Attach screenshots only when they do not show sensitive data.

### What not to enter

- Do not enter real payroll.
- Do not enter real SSNs.
- Do not enter real bank account numbers.
- Do not enter real EINs.
- Do not enter live API keys.
- Do not submit real ACH.
- Do not submit real tax filings.

Final readiness decision:

- [ ] Ready to send to testers.
- [ ] Not ready. Manual review items remain.

Reviewer notes:

________________________________________________________________________________

________________________________________________________________________________
