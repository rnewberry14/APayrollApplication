# Release Notes: Local Prototype Mode

Date: 2026-05-19

## What changed

- Added Local Prototype Mode for development and prototype testing.
- The application now uses SQL Server LocalDB in development by default.
- Added `PrototypeMode` configuration with local database naming.
- Added startup logging for Local Prototype Mode.
- Added documentation for help, admin, support, and technical audiences.
- Added unit tests for prototype mode configuration behavior.

## Why it matters

This change keeps payroll data on the user's local machine and avoids remote database storage for MVP testing.

## Notes

- LocalDB is used only in Development or when prototype mode is explicitly enabled in non-production environments.
- No telemetry, analytics, or cloud storage is added.
- ACH submission remains fake/sandbox-only by default.
