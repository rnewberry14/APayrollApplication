# Production Deployment Checklist

Use this checklist before deploying to production.

## Pre-deployment verification

- [ ] All tests pass locally: `dotnet test`
- [ ] Build succeeds locally: `dotnet build --configuration Release`
- [ ] No hardcoded secrets in code or appsettings files
- [ ] Connection string is not in appsettings; only placeholder
- [ ] API keys are not in appsettings; only placeholder
- [ ] `ACHApi:AllowProductionSubmission` is NOT enabled in non-production environments
- [ ] `ACHApi:SandboxMode` is verified for each environment
- [ ] All required GitHub secrets are configured
- [ ] All required Azure resources exist and are accessible

## Azure resource checklist

- [ ] Azure App Service created with correct .NET runtime
- [ ] Azure SQL Database created and accessible
- [ ] Azure Key Vault created with all required secrets
- [ ] Application Insights resource created
- [ ] App Service has Key Vault references configured
- [ ] App Service has connection string configured
- [ ] AZURE_KEY_VAULT_URI is set in App Service settings

## Deployment checklist

- [ ] Code committed to `main` branch
- [ ] GitHub Actions workflow triggered or manually dispatched
- [ ] Workflow completes without errors
- [ ] Application starts successfully in App Service
- [ ] Health check endpoint responds
- [ ] Application Insights telemetry is flowing
- [ ] Database migrations (if any) have been applied manually beforehand

## Post-deployment verification

- [ ] Login to the app works
- [ ] Payroll calculations execute correctly
- [ ] No errors in Application Insights
- [ ] ACH sandbox mode is confirmed (not production submission)
- [ ] All users can access their appropriate data

## Rollback plan

If the deployment fails:

1. Stop the App Service deployment: Cancel any in-progress deployment in Azure portal.
2. Revert to previous version: Use Azure App Service deployment slots or revert to previous release.
3. Check logs in Application Insights.
4. Address the root cause.
5. Retry deployment or escalate to engineering.
