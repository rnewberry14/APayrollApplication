# Payroll Run Wizard - Admin Guide

## Overview

The Payroll Run Wizard is available at `/payroll/create` in Development, Local Prototype Mode, or Local-Only Mode. It creates draft payroll runs only.

## Required Access

- View active companies.
- View active pay schedules.
- View active employees.
- Create draft payroll runs.

## Admin Notes

- Payroll mode is recorded in the audit description for the draft creation event.
- Employee notes are stored as zero-dollar earning line notes for local review.
- Overtime hours are captured as a placeholder earning line for hourly employees.
- Taxes are not calculated on this page.
- ACH and tax filing are not submitted from this page.

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Do not configure production provider calls for the local wizard workflow.
