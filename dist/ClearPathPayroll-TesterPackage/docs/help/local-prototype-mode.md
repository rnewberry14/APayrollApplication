# Local Prototype Mode

## What this feature does

Local Prototype Mode keeps payroll data on the user's machine during development and early testing. It uses a local SQL Server LocalDB database instead of remote database services.

## Who can use it

- Developers building ClearPath Payroll locally
- MVP testers who need a local-only prototype environment
- Support staff validating local deployment behavior

## How to use it

1. Run the application in the Development environment.
2. The application will automatically enable Local Prototype Mode.
3. Payroll data is stored locally in SQL Server LocalDB.
4. No cloud database, telemetry, analytics, or remote storage is used by default.

## Required permissions

- Local machine access to SQL Server LocalDB
- Permission to write to the local machine for LocalDB data storage

## Common errors

- `LocalDB instance was not found` — LocalDB is not installed or not available.
- `Permission denied` — application cannot write to the local database.
- `Could not open database` — the LocalDB database may be corrupted or locked.

## Troubleshooting

- Confirm LocalDB is installed on the local machine.
- Restart the `SqlLocalDB` service or instance.
- Reset local data by deleting the local prototype database in SQL Server LocalDB.
- Check startup logs for `Local Prototype Mode enabled.` and the database name.

## Security cautions

- Do not send payroll or bank data to external services unless sandbox integrations are explicitly enabled.
- Do not store API keys in source code.
- Keep Local Prototype Mode data on the local machine only.
