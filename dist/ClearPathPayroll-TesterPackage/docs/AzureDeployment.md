# Azure Deployment

This document explains how the ClearPath Payroll application deploys to Azure App Service using GitHub Actions.

## What this feature does

It provides a repeatable pipeline for restoring, building, testing, publishing, and deploying the payroll web app to Azure.

## Who can use it

- Developers
- DevOps engineers
- Administrators

## Key Azure resources

- Azure App Service for the web application
- Azure SQL Database for payroll data
- Azure Key Vault for production secrets
- Application Insights for monitoring

## Deployment process

1. Push changes to `main` or trigger the workflow manually.
2. The pipeline restores NuGet packages.
3. It builds the solution in Release mode.
4. It runs unit tests.
5. It publishes the web app.
6. It deploys to Azure App Service.

## Secrets and secure configuration

- Use GitHub secrets, not committed config files.
- Do not store real connection strings or API keys in source control.
- Production secrets should come from Azure Key Vault or App Service configuration.

## Database migration strategy

For MVP, production database migrations should be run manually or by a controlled release process. Do not enable destructive automatic migrations in production.
