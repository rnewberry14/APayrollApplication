# ClearPath Payroll

ClearPath Payroll is a C#/.NET payroll SaaS application.

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full SQL Server)

## Running Locally

1. Clone the repository.
2. Navigate to the `src/ClearPathPayroll` directory.
3. Restore packages: `dotnet restore`
4. Update the connection string in `appsettings.json` if needed.
5. Run database migrations: `dotnet ef database update` (once entities are added)
6. Run the application: `dotnet run`
7. Open https://localhost:5001 in your browser.

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