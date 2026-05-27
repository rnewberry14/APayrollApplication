# QuickBooks Online Import Admin Notes

The QuickBooks Online import workflow provides local template mapping for files exported from QuickBooks Online.

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## Required Permissions

- Access to import screens
- Local file read access
- Local database write access through the application

## Administration

Use `/import/quickbooks-online` to import locally saved QBO export files. Select the template that matches the exported report and review mapped aliases before confirmation.

## Safety Notes

- OAuth is not implemented.
- No direct QuickBooks Online API call is made.
- No Intuit credentials are requested or stored.
- No external file transmission is performed.
- No tax or compliance advice is provided.
