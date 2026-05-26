# Payroll Run Wizard - User Guide

## Overview

The Payroll Run Wizard creates a draft payroll run at `/payroll/create`. It supports regular payroll, after-the-fact payroll, and correction payroll workflows for local review.

The wizard does not calculate taxes, submit ACH, file taxes, call production APIs, or store data outside the local database.

## Steps

1. Select a company.
2. Select a pay schedule.
3. Select payroll mode: Regular payroll, After-the-fact payroll, or Correction payroll.
4. Confirm pay period start, pay period end, and pay date.
5. Select employees.
6. Enter pay data: regular hours, overtime hours placeholder, manual gross adjustment, and notes.
7. Click Create Payroll Run.
8. The app opens Payroll Preview for the new draft run.

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- Confirm payroll dates and entries before processing payroll.
- Do not use this page to submit ACH or tax filings.
