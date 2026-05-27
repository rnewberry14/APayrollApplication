# Creating Demo Data

## What this page does

The demo data page creates fake company, employee, pay schedule, payroll, and fake banking token records for testing.

## When to use it

Use it before testing payroll workflows so you do not need to type all setup data manually.

## Before you begin

Start the app and confirm local demo mode is visible.

## Step-by-step instructions

1. Open `/demo/seed-data`.
2. Read the warning on the page.
3. Click `Create Demo Data`.
4. Wait for the success message.
5. Open employer, employee, and pay schedule pages to confirm records exist.
6. To reset, close the app and run `RESET-DEMO-DATA.bat`.
7. Start the app again and recreate demo data.

## What to check

- Demo Company LLC exists.
- Demo employees exist.
- A demo pay schedule exists.
- Fake bank tokens or last-four values are used.

## Common messages or errors

- Create button unavailable: confirm local demo mode is enabled.
- Duplicate data: clear demo data, then create it again.

## What this page does not do

It does not create real payroll, real bank accounts, or real tax filings.

## Privacy/safety notes

Do not replace demo values with real SSNs, EINs, bank numbers, or live payroll data.

## Related articles

- [Employer Setup](employer-setup.md)
- [Employee Setup](employee-setup.md)
- [Creating a Payroll Run](creating-payroll-run.md)
