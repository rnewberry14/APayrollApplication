# Payroll SaaS Agent Instructions

This repository contains a C#/.NET payroll SaaS application.

## Technology Stack

- ASP.NET Core
- Blazor
- Entity Framework Core
- SQL Server / Azure SQL
- xUnit for tests
- External payroll tax API integration
- External ACH/direct deposit API integration
- Azure App Service deployment

## Product Goal

Build a payroll application that allows users to:
- Set up companies
- Set up employees
- Configure pay schedules
- Enter hours or salary amounts
- Calculate payroll
- Calculate employee and employer taxes through external tax APIs
- Preview payroll before approval
- Approve payroll
- Submit direct deposit through an external ACH API
- Generate pay stubs
- Generate payroll registers
- Track tax liabilities
- Maintain strong audit logs

## Important Rules

1. Do not hard-code live tax rates unless creating sample or test data.
2. Keep payroll logic separate from UI logic.
3. Keep external API logic behind interfaces.
4. Never store API keys in source code.
5. Never log SSNs, full bank account numbers, or API secrets.
6. Use dependency injection.
7. Use async methods for database and API operations.
8. Add unit tests for calculation logic.
9. Add audit logs for important payroll actions.
10. Use clear, readable code suitable for a beginner owner to understand.

## Payroll Safety Rules

Payroll calculations must be deterministic and auditable.

Every payroll run should preserve:
- gross wages
- employee taxes
- employer taxes
- deductions
- net pay
- direct deposit amount
- calculation timestamp
- tax API version or response reference if available
- user who approved payroll

Payroll approval must lock the payroll run from casual edits.

## External API Rules

For tax APIs and ACH APIs:
- Create an interface first.
- Create a sandbox implementation before production.
- Create fake/mock versions for testing.
- Do not call production APIs in tests.
- Do not submit real ACH files or payments in development.
- Log API failures without exposing sensitive data.

## Testing Rules

For every service:
- Add unit tests.
- Include normal cases.
- Include edge cases.
- Include invalid input cases.
- Include rounding tests where money is involved.

## Security Rules

Protect:
- SSNs
- bank account numbers
- routing numbers
- employee addresses
- wage data
- employer tax IDs
- API keys

Use:
- role-based access
- audit logs
- encryption where appropriate
- Azure Key Vault or equivalent for secrets

## Coding Style

- Use simple, readable C#.
- Prefer small classes and services.
- Add comments for payroll-specific logic.
- Avoid clever code.
- Prefer clarity over complexity.