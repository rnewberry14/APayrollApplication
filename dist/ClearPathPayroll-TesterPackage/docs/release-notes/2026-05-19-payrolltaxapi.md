# Release Notes: PayrollTaxAPI Sandbox Integration

Date: 2026-05-19

## What changed

- Added a new sandbox integration service for PayrollTaxAPI.com.
- Implemented `PayrollTaxApiCalculationService` using `HttpClientFactory`.
- Added request/response mapping between internal tax models and the vendor API.
- Added safe error handling and safe logging.
- Added unit tests for vendor response mapping, HTTP failures, and sandbox headers.
- Added an integration test placeholder that is skipped by default.
- Added documentation for help, admin, support, and technical audiences.

## Why it matters

This release enables the payroll application to calculate taxes through an external sandbox API while preserving security and audit requirements.

## Notes

- API keys must be kept out of source control.
- Sandbox mode should be used for development and staging only.
