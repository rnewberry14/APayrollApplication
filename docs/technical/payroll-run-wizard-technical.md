# Payroll Run Wizard - Technical Notes

## Implementation

- Page: `src/ClearPathPayroll/Components/Pages/PayrollRunCreate.razor`
- Route: `/payroll/create`
- Service: `PayrollService.CreateDraftPayrollRunAsync`
- Tests: `PayrollServiceTests` and `PayrollRunWizardPageTests`

## Behavior

The page uses a six-step Blazor wizard:

1. Company selection.
2. Pay schedule selection.
3. Payroll mode selection.
4. Pay date confirmation.
5. Employee selection.
6. Pay data entry.

The service creates a `PayrollRun` in Draft status, `PayrollRunEmployee` records, regular earning lines, optional overtime placeholder lines, optional manual gross adjustment lines, optional note lines, and an audit log entry.

## Boundaries

- No tax calculation is run from this page.
- No ACH submission is run from this page.
- No tax filing is run from this page.
- No production API call is made by this workflow.

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
