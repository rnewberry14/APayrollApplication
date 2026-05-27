# Local Prototype Mode Technical Notes

## Feature summary

Local Prototype Mode enables local-only data storage for ClearPath Payroll during development and early testing. It uses SQL Server LocalDB so payroll and related data remain on the developer's machine.

## Architecture

- The application detects `Development` environment and enables local prototype mode automatically.
- Local prototype mode uses SQL Server LocalDB with a database name configured in `PrototypeMode:LocalDbDatabaseName`.
- The local database is kept on the user's machine and not stored in the cloud.
- No telemetry, analytics, or cloud storage is configured for prototype mode.

## Configuration

### appsettings.json

```json
"PrototypeMode": {
  "Enabled": false,
  "LocalDbDatabaseName": "ClearPathPayroll.LocalPrototype"
}
```

### appsettings.Development.json

```json
"PrototypeMode": {
  "Enabled": true,
  "LocalDbDatabaseName": "ClearPathPayroll.LocalPrototype.Dev"
}
```

## Implementation details

- `PrototypeModeHelper` decides when local prototype mode is active.
- The application uses LocalDB when running in `Development` or when prototype mode is explicitly enabled in a non-production environment.
- Startup logs record `Local Prototype Mode enabled.` without exposing sensitive data.
- The default remote connection string is still available for non-prototype environments.

## Reset local data

- Use SQL Server LocalDB tools to delete the local prototype database.
- Restart the application to recreate the local database schema.
- No cloud services are involved during reset.

## Security cautions

- Do not send payroll, employee, SSN, bank, or tax data to external services unless sandbox integrations are explicitly enabled.
- Do not enable production data flows in prototype mode.
- Keep API keys out of source control and use sandbox providers only when explicitly configured.
