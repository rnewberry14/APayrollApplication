# Azure Deployment

## What this feature does

The Azure Deployment workflow builds, tests, publishes, and deploys ClearPath Payroll to Azure App Service.

## Who can use it

- Developers
- Release managers

## Steps

1. Commit code to the `main` branch or trigger the deployment workflow manually in GitHub.
2. Ensure required GitHub secrets are configured.
3. The workflow restores packages, builds in Release mode, runs tests, and publishes the app.
4. The workflow deploys the published app to Azure App Service.

## Required permissions

- GitHub repository write access to trigger workflows.
- Azure App Service contributor or deployment permissions for admins.

## Common errors

- Pipeline fails because tests failed.
- Deployment cannot complete because Azure publish profile or service principal secrets are missing.
- Production deployment fails because required Azure App Service settings or Key Vault references are missing.

## Troubleshooting

- Check the workflow run details in GitHub Actions.
- Confirm Azure secrets are configured in repository secrets.
- Confirm the correct `main` branch is used.
- Verify Azure App Service and Key Vault are available.

## Security caution

> Caution: Do not commit publish profiles, API keys, connection strings, or secrets into Git. Use GitHub secrets and Azure Key Vault instead.
