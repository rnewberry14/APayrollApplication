# ClearPath Payroll Tester README

Tester package only. Do not use for real payroll. Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.

## What This Demo Is

ClearPath Payroll is a local demo/test application for manual review of payroll setup, payroll run, reporting, import, pay stub, and fake/sandbox direct deposit workflows.

The demo runs on your Windows computer and opens in your local browser. Test data stays local to the extracted demo folder unless you choose to move or share files yourself.

## What This Demo Is Not

This demo is not a live payroll system. It does not submit real ACH, submit real tax filings, transmit production provider data, or provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.

## Safety Warnings

- Do not use for real payroll.
- Do not enter real SSNs.
- Do not enter real bank account numbers.
- Do not enter real EINs.
- Do not enter live API keys.
- Do not submit real ACH.
- Do not submit real tax filings.
- Use fake/demo/sandbox data only.

## System Requirements

- Windows computer.
- Local browser such as Microsoft Edge, Chrome, or Firefox.
- .NET runtime may be required if this package was built as framework-dependent. If the app does not start, see `TROUBLESHOOTING-TESTER-STARTUP.md`.

## Start The App

1. Unzip the tester package.
2. Open the extracted folder.
3. Double-click `START-ClearPathPayroll.bat`.
4. Wait for the app window to start.
5. Your browser should open to `http://localhost:5080`.

If the browser does not open, manually open your browser and go to:

```text
http://localhost:5080
```

If the launcher says the app did not answer within 60 seconds, open `ClearPathPayroll-startup.log` in the extracted folder and send the error text back with your feedback.

## Seed Demo Data

1. Start the app.
2. Open `/demo/seed-data` in the browser.
3. Click `Create Demo Data`.
4. Use only the generated fake/demo records for testing.

## Reset Demo Data

1. Close the ClearPath Payroll app window.
2. Double-click `RESET-DEMO-DATA.bat`.
3. Confirm the reset when prompted.
4. Start the app again with `START-ClearPathPayroll.bat`.
5. Open `/demo/seed-data` and click `Create Demo Data`.

The reset file only removes local demo database files from the extracted application folder.

## Follow The Manual Test Packet

1. Open `MANUAL-TEST-PACKET.md`.
2. Start with `/prototype-test-checklist`.
3. Mark each item complete as you test.
4. Record issues in `TESTER-FEEDBACK-FORM.md`.

## Report Feedback

Send back:

- Completed `TESTER-FEEDBACK-FORM.md`.
- Screenshots of errors or confusing screens.
- The page or workflow name.
- The exact click path that caused the issue.

Do not send SSNs, bank information, passwords, API keys, real EINs, or live payroll data.
