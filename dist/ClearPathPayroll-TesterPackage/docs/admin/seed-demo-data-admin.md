# Seed Demo Data — Admin Guide

Overview

- The Seed Demo Data feature creates a demo company, employees, pay schedule, demo bank tokens, and a draft payroll run in the local database.

Who should use it

- Administrators and DevOps who set up developer workstations or QA environments using LocalDB.

Usage

1. Ensure the environment is `Development` or Local Prototype Mode.
2. Launch the application and use the **Seed Demo Data** page in the navigation.
3. Confirm the seed operation succeeded by inspecting the returned Company ID and Payroll Run ID.

Clearing demo data

- Use SQL Server Object Explorer or `SqlLocalDB` to delete the demo database or remove the `Demo Company (Local Prototype)` rows.

Required permissions

- Ability to run LocalDB and modify local SQL Server databases.

Common issues

- Menu not visible: verify `PrototypeMode` and environment.
- Seed operation fails: check database connectivity and application logs.

Security notes

- All demo data uses obvious fake tokens (Last4 = 0000, tokens contain "demo-").
- Do not export or share seeded payroll data.
