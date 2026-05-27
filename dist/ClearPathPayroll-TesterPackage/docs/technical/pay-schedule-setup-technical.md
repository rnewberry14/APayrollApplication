# Pay Schedule Setup - Technical Notes

## Overview

The Pay Schedule Setup page is a Blazor page at `/setup/pay-schedules`. It uses the existing `PaySchedule` entity, `PayScheduleService`, and `CompanyService`.

## Implementation Details

- Page: `src/ClearPathPayroll/Components/Pages/PayScheduleSetup.razor`
- Route: `/setup/pay-schedules`
- Entity: `ClearPathPayroll.Domain.PaySchedule`
- Services:
  - `CompanyService.GetAllActiveCompaniesAsync`
  - `PayScheduleService.GetPaySchedulesByCompanyAsync`
  - `PayScheduleService.CreatePayScheduleAsync`
  - `PayScheduleService.CalculateNextPayPeriod`

The page model uses data annotation validation and `IValidatableObject` for date relationship checks.

## Who Can Use It

The application should expose this page only to users with company setup and payroll schedule management permissions. Role enforcement should be added through the app's authentication and authorization layer as it matures.

## Required Permissions

- Read active companies.
- Read pay schedules for a selected company.
- Create pay schedules.

## Validation

- Company must be selected.
- Name is required and limited to 100 characters.
- Period end date must be on or after period start date.
- Pay date must be on or after period end date.

Confirm pay schedule dates before processing payroll.

## Common Errors and Troubleshooting

- Empty company list: verify company seed/setup data and `Company.IsActive`.
- Save failure: check database connectivity and EF Core validation.
- Unexpected preview: review `PayScheduleService.CalculateNextPayPeriod`.
- Inactive schedule unavailable elsewhere: confirm downstream services filter with `GetActivePaySchedulesByCompanyAsync`.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not log SSNs, full bank account numbers, routing numbers, employer tax IDs, API keys, or ACH secrets.
- Do not call tax or ACH production APIs from tests.
- Schedule dates should be treated as audit-relevant setup data because they affect payroll run creation and downstream processing.
