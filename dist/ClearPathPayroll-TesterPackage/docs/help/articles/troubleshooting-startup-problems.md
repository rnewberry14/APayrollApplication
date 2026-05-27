# Troubleshooting Startup Problems

## What this page does

This article helps testers resolve common local startup issues.

## When to use it

Use it when the tester package does not open, the browser does not load, or Windows shows a warning.

## Before you begin

Use the extracted tester package folder. Do not run files from inside the zip.

## Step-by-step instructions

1. Confirm the package is unzipped.
2. Double-click `START-ClearPathPayroll.bat`.
3. If the browser does not open, go to `http://localhost:5080`.
4. If the port is busy, close other app windows and try again.
5. If .NET is missing, install the .NET 8 ASP.NET Core Runtime from Microsoft.
6. If Windows shows a security warning, continue only if the package source is trusted.

## What to check

- App window stays open.
- Browser reaches the local URL.
- Local demo mode banner appears.

## Common messages or errors

- Missing .NET runtime: install runtime.
- Port already in use: close old app windows.
- App not found: confirm launcher is in the extracted package folder.

## What this page does not do

It does not troubleshoot production deployment or cloud hosting.

## Privacy/safety notes

Do not send screenshots containing SSNs, bank information, passwords, API keys, or live payroll data.

## Related articles

- [Getting Started with the Local Demo](getting-started-local-demo.md)
- [How to Report Feedback](how-to-report-feedback.md)
