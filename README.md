# ClearPath Payroll

ClearPath Payroll is a C#/.NET payroll SaaS application.

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full SQL Server)

## Running Locally

ClearPath Payroll supports a local prototype mode during development. In `Development`, the app uses SQL Server LocalDB by default and keeps payroll data on the developer's machine.

- Ensure `PrototypeMode:Enabled` is `true` in `src/ClearPathPayroll/appsettings.Development.json`.
- Local mode uses the configured `PrototypeMode:LocalDbDatabaseName`.
- The default local database is `ClearPathPayroll.LocalPrototype.Dev` in development.


1. Clone the repository.
2. Navigate to the `src/ClearPathPayroll` directory.
3. Restore packages: `dotnet restore`
4. Configure secrets locally using .NET User Secrets:
   - `dotnet user-secrets init`
   - `dotnet user-secrets set "TaxApi:ApiKey" "YOUR_DEV_TAX_API_KEY"`
   - `dotnet user-secrets set "ACHApi:ApiKey" "YOUR_DEV_ACH_API_KEY"`
   - `dotnet user-secrets set "BankVerification:ApiKey" "YOUR_DEV_BANK_VERIFICATION_API_KEY"`
   - `dotnet user-secrets set "Email:ApiKey" "YOUR_DEV_EMAIL_API_KEY"`
   - `dotnet user-secrets set "Encryption:LocalDevelopmentKey" "YOUR_DEV_ENCRYPTION_KEY"`
5. Update `appsettings.json` only for safe development defaults; do not add real secrets to source control.
6. Run database migrations: `dotnet ef database update` (once entities are added)
7. Run the application: `dotnet run`
8. Open https://localhost:5001 in your browser.

## Building a Tester Package

The tester package is for local demo/testing only.

Tester package only. Do not use for real payroll. Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.

From the repository root, run:

```powershell
.\packaging\build-tester-package.ps1
```

The script restores, builds, tests, publishes in Release mode, creates `dist\ClearPathPayroll-TesterPackage`, copies tester documentation, and creates `dist\ClearPathPayroll-TesterPackage.zip`.

The package excludes repository metadata, source build folders, local database files, uploaded/imported source files, user secrets, and files containing common secret patterns. Review `packaging\package-manifest.md` for the included and excluded file list.

## MVP Scope

The first version supports:
- Oklahoma employers
- W-2 employees
- Salary and hourly payroll
- Regular payroll runs
- Payroll preview
- Payroll approval
- Pay stubs
- Payroll register
- Tax API integration
- ACH/direct deposit integration
- Audit logs

## Not Included in MVP

- Multi-state payroll
- Garnishments
- Benefit administration
- Automatic federal/state tax filing
- Contractor payroll
- Certified payroll
