# Azure Deployment Troubleshooting

## Purpose

This guide helps support staff troubleshoot Azure deployment and pipeline failures for ClearPath Payroll.

## Common issues

- Build or test failures in the workflow.
- Missing GitHub secrets for Azure deployment.
- Azure App Service deployment errors.
- Application startup failures due to missing production settings.

## Troubleshooting steps

1. Review the GitHub Actions workflow run and logs.
2. Confirm the correct branch is deployed (`main`).
3. Verify required GitHub secrets exist and are not empty.
4. Confirm Azure App Service exists and the app name matches the secret.
5. Check App Service configuration for missing Key Vault references or connection strings.

## What to ask the customer

- Did the failure occur during build, test, publish, or deploy?
- Is the pipeline using a publish profile or a service principal?
- Has the Azure App Service name changed?
- Are production Key Vault references configured in App Service settings?

## Support-safe guidance

> Do not ask customers to share raw secrets, publish profiles, or connection strings. Ask for the names of configured resources and the specific failure step.
