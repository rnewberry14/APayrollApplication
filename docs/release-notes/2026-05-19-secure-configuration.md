# 2026-05-19 Secure Configuration

- Added secure configuration support for ClearPath Payroll.
- Configured environment-specific settings for Development, Staging, and Production.
- Added support for .NET User Secrets in local development.
- Added Azure Key Vault support for production secrets.
- Added ACH sandbox and explicit production submission guard rails.
- Added startup validation to fail safely when required production secrets are missing.
