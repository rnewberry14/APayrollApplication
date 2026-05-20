# ClearPath Payroll Copilot Instructions

This is a C#/.NET payroll SaaS application.

## Required documentation behavior

Whenever you create, modify, or remove a feature, you must also update the matching documentation.

For each feature, update or create:

1. End-user help article in `/docs/help/`
2. Admin guide in `/docs/admin/`
3. Support troubleshooting guide in `/docs/support/`
4. Technical implementation note in `/docs/technical/`
5. Release note entry in `/docs/release-notes/`

## Documentation style

Write documentation for non-technical payroll users.

Use:
- short sections
- plain English
- step-by-step instructions
- screenshots placeholders when useful
- disclaimer statements
- warnings for payroll, tax, ACH, or security-sensitive actions
- helpful hints, notes, and cautions to make user aware of dos and don'ts to avoid confusion
- “What this does”
- “Before you begin”
- “Steps”
- “Common errors”
- “What to check if something goes wrong”

Avoid:
- unexplained developer jargon
- legal advice
- legal guarantees
- saying the system guarantees tax compliance
- tax advice
- using language that can be implied as tax or legal advice or guarantees
- exposing secrets, SSNs, bank account numbers, or API keys

## Payroll documentation warnings

Any feature involving payroll approval, taxes, direct deposit, bank accounts, employee tax setup, ACH submission, or tax reports must include a caution box.

Example:

> Caution: Review payroll totals carefully before approval. Approved payroll may create tax liabilities and direct deposit obligations.

Any language that can be construed as legal or tax advice must include a disclaimer statement or box.

Example:

> Disclaimer: ClearPath Payroll is not an attorney, tax practitioner, or CPA and does not provide any information that can or should be considered as official advice or guidance. Anything that could be considered as such should be verified by the users. ClearPath Payroll strongly recommends that you consult with the appropriate government agency, your attorney or your tax advisor for official guidance. 

## Required documentation checklist

Every completed issue should include:

- [ ] User help article updated
- [ ] Admin guide updated
- [ ] Support troubleshooting guide updated
- [ ] Technical note updated
- [ ] Release note updated
- [ ] Any new settings documented
- [ ] Any new permissions documented
- [ ] Any new error messages documented