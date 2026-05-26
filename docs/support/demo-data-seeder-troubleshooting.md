# Demo Data Seeder - Troubleshooting

## Overview

Use this guide when `/demo/seed-data` cannot create or clear fake demo records.

## Common Issues

| Issue | Cause | Resolution |
| --- | --- | --- |
| Page says seeding is unavailable | App is not in Development or Local-Only Mode | Enable local-only mode or run with `ASPNETCORE_ENVIRONMENT=Development` |
| Create Demo Data says records already exist | Demo Company LLC and the required related demo records are already present | Continue testing or click Clear Demo Data first |
| Create Demo Data says records were repaired | A previous seed created only part of the demo dataset | Continue testing; the missing demo records were recreated |
| Create Payroll has no employees or schedule after seeding | Browser was on the older seed route or a partial seed existed | Open `/demo/seed-data` and click Create Demo Data again |
| Button appears but clicking does nothing | Blazor interactivity is not active or the app was built before the interactive routing fix | Rebuild and run the app, then confirm `/demo/seed-data` loads with `_framework/blazor.web.js` |
| App does not start before the seed page loads | SQL Server LocalDB instance is missing or broken | Use the default SQLite local-only setting and run the app again |
| Clear Demo Data says no records found | Demo Company LLC is not present | Click Create Demo Data |
| Database error | Local database is unavailable or migrations have not run | Restart the app so local migrations run |

## Safety Checks

- Confirm no full SSN appears in the database.
- Confirm bank values are fake tokens, not routing or account numbers.
- Confirm no tax API or ACH API calls are made from the page.

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Demo data is fake and must not be used for real payroll.
