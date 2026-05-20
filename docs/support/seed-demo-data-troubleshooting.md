# Seed Demo Data — Support Troubleshooting

Symptoms

- Seed page missing from navigation.
- Seed action fails with database errors.
- Seed completes but expected rows not present.

Checks

- Confirm `ASPNETCORE_ENVIRONMENT` is `Development` or `PrototypeMode:Enabled` is true.
- Confirm LocalDB is installed and running (`sqllocaldb info`).
- Check application logs for exceptions during seeding.

Fixes

- If LocalDB not installed, install SQL Server Express LocalDB.
- If permission issues, ensure the user can create databases.
- To remove seeded data, either delete the demo database via SQL Server tools or remove rows where `Company.LegalName = 'Demo Company (Local Prototype)'.`

Security cautions

- Do not attempt to connect seeded demo data to production integrations (ACH, Tax APIs) — demo bank tokens are not real.
