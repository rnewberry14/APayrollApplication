# Seed Demo Data

What it does

- Provides a development-only tool to populate the local database with sample payroll data for testing and demos.

Who can use it

- Developers and QA running the application in `Development` or Local Prototype Mode.

Step-by-step

1. Run the application in `Development` (ASPNETCORE_ENVIRONMENT=Development) or enable `PrototypeMode` in your non-production config.
2. Open the app in a browser at `https://localhost:5001`.
3. From the navigation menu select **Seed Demo Data** (visible only in Development or Local Prototype Mode).
4. Click the **Seed Demo Data** button and wait for the confirmation message.

Required permissions

- Local user account must have permission to create and write to the LocalDB database used by the app.

Common errors

- "Demo data is only available in Development or Local Prototype Mode." — You're not in an allowed environment.
- Database connection errors — ensure LocalDB is installed and accessible.

Troubleshooting

- Confirm `PrototypeMode:Enabled` is true in `appsettings.Development.json` if you expect the menu to appear.
- Use SQL Server Object Explorer or `SqlLocalDB` to inspect or remove seeded data.

Security cautions

- Seeded data contains fake values only. No real SSNs, bank numbers, or tax IDs are used.
- Do not use seeded demo data for production testing.
