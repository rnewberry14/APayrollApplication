# Local Prototype Mode Admin Guide

## Feature overview

Local Prototype Mode configures the application to use a local SQL Server LocalDB database during development and prototype testing.

## Who should use it

- Administrators setting up development or prototype environments
- DevOps engineers validating local-only behavior

## Setup steps

1. Ensure the local machine has SQL Server LocalDB installed.
2. Verify the application is running in the Development environment.
3. Confirm `PrototypeMode:Enabled` is enabled in `appsettings.Development.json`.
4. The database name is configured in `PrototypeMode:LocalDbDatabaseName`.

## Required permissions

- Permission to run LocalDB on the local machine
- Permission to create and access local SQL Server databases

## Common administrative issues

- LocalDB is not installed or configured.
- The development environment is not recognized as `Development`.
- `PrototypeMode:Enabled` is incorrectly disabled in development configuration.

## Troubleshooting

- Verify the environment name in launch settings or application host.
- Confirm the `PrototypeMode` section is present in configuration.
- Check that `Local Prototype Mode enabled.` appears in startup logs.
- Use SQL Server Object Explorer or `SqlLocalDB` CLI to inspect the local database.

## Security notes

- This mode is intentionally local only.
- Do not enable remote database services for prototype mode.
- Do not log sensitive payroll data or secrets.
