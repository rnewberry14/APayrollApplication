# Azure Deployment Admin Guide

## What this feature does

This guide explains how to configure Azure resources and GitHub Actions for ClearPath Payroll deployment.

## Who can use it

- System administrators
- DevOps engineers
- Infrastructure owners

## Required permissions

- Azure subscription contributor or owner
- Azure App Service deployment permissions
- Azure SQL Database contributor
- Azure Key Vault contributor or access policy permissions
- GitHub repository admin or workflow permissions

## Required Azure resources

- Azure App Service web app
- Azure SQL Database server and database
- Azure Key Vault for production secrets
- Application Insights resource for telemetry

## Required GitHub secrets

- `AZURE_WEBAPP_NAME`
- `AZURE_WEBAPP_PUBLISH_PROFILE` or service principal secrets:
  - `AZURE_CLIENT_ID`
  - `AZURE_TENANT_ID`
  - `AZURE_SUBSCRIPTION_ID`
- Optional: `AZURE_KEY_VAULT_URI`

## Deployment configuration

1. Create the App Service and SQL Database in your Azure subscription.
2. Configure Key Vault and add production secrets.
3. Add GitHub secrets for deployment credentials.
4. Confirm `ACHApi:AllowProductionSubmission` is enabled only in true production.
5. Use `docs/ProductionDeploymentChecklist.md` before each production release.

## Troubleshooting common issues

- If deployment fails, verify the publish profile or service principal secrets.
- If application startup fails, check App Service settings and Key Vault references.
- If database connectivity fails, verify the SQL connection string is set in App Service configuration.

## Security caution

> Caution: Production secrets must never be stored in Git or appsettings files. Use Azure Key Vault and App Service application settings.
