# End-to-End Prototype Test Checklist - Technical Notes

## Overview

The End-to-End Prototype Test Checklist is a Blazor page for local prototype test coordination. It is implemented as a UI-only workflow and does not change payroll, tax, ACH, or calculation logic.

## Implementation Details

- Page: `src/ClearPathPayroll/Components/Pages/PrototypeTestChecklist.razor`
- Route: `/prototype-test-checklist`
- Navigation: `src/ClearPathPayroll/Components/Layout/NavMenu.razor`
- Prototype guard: `PrototypeModeHelper.ShouldUseLocalPrototypeMode`
- Disclaimer: `NoAdviceDisclaimer`

Checklist state is stored in an in-memory dictionary in the Blazor component. It is not persisted to the database and is reset when the page reloads or when the tester clicks Reset Checklist.

## Who Can Use It

Users in Development or Local Prototype Mode who are allowed to exercise the prototype payroll workflow.

## Required Permissions

- Open local prototype UI pages.
- Seed demo data.
- Create and approve local prototype payroll runs.
- View local prototype reports and pay stubs.

## Step-by-Step Technical Flow

1. The route renders `PrototypeTestChecklist.razor`.
2. The page calls `PrototypeModeHelper.ShouldUseLocalPrototypeMode`.
3. If prototype mode is not available, checkbox controls are disabled.
4. The page renders the standard no-advice disclaimer.
5. The page renders static checklist items and optional links to existing prototype routes.
6. Checkbox changes update only component memory.

## Common Errors

- Disabled controls indicate the environment is not Development or Local Prototype Mode.
- Missing Company Setup or Employee Setup routes should not be solved from this checklist page; those steps are manual review steps until dedicated setup pages exist.
- Pay stub navigation requires a payroll run ID and employee ID.

## Troubleshooting Steps

- Verify `ASPNETCORE_ENVIRONMENT` is Development for local testing.
- If testing a non-production environment, verify prototype mode is explicitly enabled.
- Confirm `NoAdviceDisclaimer` remains visible on the page.
- Run `dotnet test` and confirm `PrototypeTestChecklistPageTests` passes.

## Payroll, Tax, ACH, and Security Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- This page must not call production APIs.
- This page must not submit ACH or tax filings.
- This page must not display full SSNs, routing numbers, full bank account numbers, employer tax IDs, or API secrets.
