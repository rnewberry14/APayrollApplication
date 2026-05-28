# Employee Import Name Mapping

Date: 2026-05-28

## Changed

- Added `FullName` as an employee import target field.
- Employee imports now accept either `FirstName` plus `LastName`, or `FullName`.
- Added common `FullName` aliases, including `Worker Name`.
- Added conservative full-name parsing for common formats.
- Added validation messages when a full name needs review.

## Safety

This workflow remains local-only. It does not upload files, submit ACH, file taxes, or store full SSNs.
