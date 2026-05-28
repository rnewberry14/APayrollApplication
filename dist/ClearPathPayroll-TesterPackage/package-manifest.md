# ClearPath Payroll Tester Package Manifest

## App

- App name: ClearPath Payroll
- Version: tester build placeholder
- Build date: 2026-05-27 15:59:37 UTC
- Package type: local demo/test application

## Safety Warnings

Tester package only. Do not use for real payroll. Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.

ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice. This package uses fake/demo/sandbox data only and does not submit real ACH or real tax filings.

## Included Files

- Published ClearPath Payroll application files from `dotnet publish`.
- Runtime files required by the published ASP.NET Core application.
- Tester-facing documentation copied from `/docs`.
- Help Center source articles under `docs/help/articles`.
- Testing packet files under `docs/testing`, including the final tester package checklist.
- `README-TESTERS.md`.
- `START-ClearPathPayroll.bat`.
- `RESET-DEMO-DATA.bat`.
- `TROUBLESHOOTING-TESTER-STARTUP.md`.
- `MANUAL-TEST-PACKET.md`, when present.
- `TESTER-FEEDBACK-FORM.md`, when present.
- `package-manifest.md` with the generated build date.
- This manifest.

## Excluded Files

- `.git` repository metadata.
- Source `bin` and `obj` build folders outside the publish output.
- .NET User Secrets.
- Appsettings files containing real secrets.
- Local database files with real or test-entered data.
- Uploaded PDFs.
- Imported source files such as CSV, TSV, TXT, XLS, or XLSX files.
- API keys, passwords, tokens, private keys, and live provider credentials.
- Production provider transmission files.
- Telemetry or analytics configuration files.

## Known Limitations

- Local tester package only.
- Uses fake/demo/sandbox workflows only.
- No production ACH submission.
- No production tax filing.
- No telemetry.
- No hosted database dependency is configured by default for the tester package.
- PDF import supports local text extraction only; scanned/image-only PDFs may require manual entry or spreadsheet import.
- Imported payroll and W-2 data is user-provided historical/demo data and requires review before saving.

## Tester Launch Files

- `START-ClearPathPayroll.bat` starts the local app on `http://localhost:5080`.
- `RESET-DEMO-DATA.bat` removes local demo database files from the extracted app folder only.
- `TROUBLESHOOTING-TESTER-STARTUP.md` explains startup, browser, port, runtime, and Windows security warning troubleshooting.

## Packaging Notes

The Windows packaging script creates `/dist/ClearPathPayroll-TesterPackage` and `/dist/ClearPathPayroll-TesterPackage.zip`. The script scans packaged files for common secret patterns and blocks package creation when a risky value is detected.

