# 2026-05-21 - No-Advice Guardrails

## Summary

Added standard no-advice disclaimer language across key ClearPath Payroll UI and documentation surfaces.

## Changes

- Added reusable `NoAdviceDisclaimer` component.
- Displayed the disclaimer on Payroll Preview, approval confirmation, fake/sandbox direct deposit, and Tax Liability Report.
- Added the disclaimer to PayrollTaxAPI integration documentation.
- Replaced risky documentation wording that implied compliance determinations, professional guidance, or legal/labor-law conclusions.
- Added tests that verify the standard disclaimer component text and required page usage.

## Product Boundary

ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
