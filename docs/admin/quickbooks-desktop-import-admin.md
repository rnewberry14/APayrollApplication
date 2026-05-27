# QuickBooks Desktop Import Admin Notes

The QuickBooks Desktop import workflow provides local template mapping for exported reports. It stores staged import batches in the local database.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Required Permissions

- Access to import screens
- Local file read access
- Local database write access through the application

## Administration

Use `/import/quickbooks-desktop` to import exported report files. Select the template that matches the source report and review mapped aliases before confirmation.

## Safety Notes

- No QuickBooks Desktop SDK is used.
- No direct QuickBooks Desktop connection is made.
- No Intuit credentials are requested or stored.
- No external file transmission is performed.
- No tax or compliance advice is provided.
