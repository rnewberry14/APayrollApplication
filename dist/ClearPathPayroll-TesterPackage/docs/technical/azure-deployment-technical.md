# Azure Deployment Technical Notes

## What this feature does

The Azure Deployment workflow is a GitHub Actions pipeline that builds, tests, publishes, and deploys ClearPath Payroll to Azure App Service.

## Workflow structure

- **Trigger**: Push to `main` or manual dispatch
- **Build**: Restores NuGet, builds Release, runs tests, publishes
- **Deploy**: Deploys to Azure App Service using publish profile or service principal

## Configuration

- Workflow file: `.github/workflows/azure-deploy.yml`
- Solution: `ClearPathPayroll.sln`
- Web project: `src/ClearPathPayroll/ClearPathPayroll.csproj`
- Test project: `tests/ClearPathPayroll.Tests/ClearPathPayroll.Tests.csproj`

## Deployment methods

1. **Publish Profile** (simpler):
   - Set `AZURE_WEBAPP_PUBLISH_PROFILE` secret with publish profile content
   - Set `AZURE_WEBAPP_NAME` secret with the app name

2. **Service Principal** (recommended for production):
   - Set `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`
   - Set `AZURE_WEBAPP_NAME`

## Security

- Secrets are never logged
- Production connection strings come from Azure Key Vault or App Service settings
- ACH production submission requires explicit enable flag in App Service config

## Database migrations

Manual or controlled process. Do not run destructive migrations automatically.
