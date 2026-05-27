# Local Prototype Mode Troubleshooting

## What this feature does

Local Prototype Mode keeps application data on the developer's local machine using SQL Server LocalDB.

## Who should use this guide

- Support engineers troubleshooting local development issues
- QA testers validating local-only behavior
- Administrators diagnosing prototype environment problems

## Step-by-step troubleshooting

1. Confirm the application is running in the Development environment.
2. Verify LocalDB is installed and accessible on the local machine.
3. Look for startup logs that say `Local Prototype Mode enabled.`
4. Verify `PrototypeMode:Enabled` is set in the development configuration.

## Common errors

- `LocalDB instance was not found` — install SQL Server LocalDB.
- `Could not open database` — the local database may be unavailable or locked.
- `Permission denied` — the application cannot create or open the local database.

## Recommended fixes

- Restart `SqlLocalDB` or the local machine.
- Confirm the development launch profile uses `ASPNETCORE_ENVIRONMENT=Development`.
- If the database is corrupted, delete the local LocalDB database and restart.

## Security cautions

- Do not move local payroll data to external systems without explicit configuration.
- Do not enable telemetry or analytics for local prototype mode.
- Do not use real ACH submission in prototype mode.
