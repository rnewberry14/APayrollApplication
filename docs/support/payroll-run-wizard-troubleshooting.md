# Payroll Run Wizard - Troubleshooting

## Common Issues

| Issue | Likely Cause | Resolution |
| --- | --- | --- |
| No company appears | No active company exists | Create or seed demo company data |
| No pay schedule appears | Selected company has no active schedule | Create a pay schedule for the company |
| No employees appear | Selected company has no active employees | Create or seed employees |
| Cannot continue from employee step | No employees are selected | Select at least one employee |
| Create fails | Negative hours or adjustment values were entered | Enter zero or positive values |

## Cautions

- ClearPath Payroll provides software tools for payroll calculation, reporting, and data transmission. It does not provide legal, tax, financial, accounting, or payroll compliance advice. Users are responsible for verifying payroll, tax, filing, payment, and compliance requirements with qualified professionals or official agency sources.
- This wizard does not submit ACH, file taxes, or call production APIs.
