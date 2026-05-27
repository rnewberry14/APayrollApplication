# Getting Started with the Local Demo

## What this page does

This article explains how to open the ClearPath Payroll tester package and begin manual review of the local demo application.

## When to use it

Use this first, before testing setup, payroll, reports, imports, or printing.

## Before you begin

Have the unzipped tester package on a Windows computer. Use fake/demo data only.

## Step-by-step instructions

1. Open the extracted tester package folder.
2. Read `README-TESTERS.md`.
3. Double-click `START-ClearPathPayroll.bat`.
4. Wait for the app window to start.
5. Open `http://localhost:5080` if the browser does not open automatically.
6. Open `/prototype-test-checklist`.
7. Open `/demo/seed-data` and create demo data.

## What to check

- The browser opens the local app.
- A local/demo safety message is visible.
- The app does not ask for real payroll data.

## Common messages or errors

- Missing .NET runtime: install the .NET 8 ASP.NET Core Runtime from Microsoft.
- Port already in use: close old app windows and start again.
- Browser does not open: manually go to `http://localhost:5080`.

## What this page does not do

This article does not configure production payroll, tax filing, ACH, or cloud services.

## Privacy/safety notes

Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.

## Related articles

- [Local Demo Mode Explained](local-demo-mode-explained.md)
- [Creating Demo Data](creating-demo-data.md)
- [Troubleshooting Startup Problems](troubleshooting-startup-problems.md)
