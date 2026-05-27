# No-Advice Guardrails - Technical Notes

## Overview

ClearPath Payroll uses a standard disclaimer component and documentation language to clarify the product boundary.

## Standard Disclaimer

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.

## UI Placement

- Payroll Preview
- Payroll approval confirmation context
- Fake/sandbox direct deposit submission context
- Tax Liability Report

## Implementation Details

- Component: `src/ClearPathPayroll/Components/Shared/NoAdviceDisclaimer.razor`
- Shared import: `src/ClearPathPayroll/Components/_Imports.razor`
- Tests: `tests/ClearPathPayroll.Tests/NoAdviceDisclaimerTests.cs`

## Maintenance Notes

- Do not describe the software as legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
- Do not use wording that says the software determines or satisfies compliance status.
- Avoid absolute compliance claims.
- Keep calculation, tax API, and ACH logic separate from disclaimer wording.
