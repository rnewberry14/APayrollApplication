# 2026-05-28 Import Mapping UX

## Summary

Improved import progress visibility and mapping usability for local file imports.

## Changes

- Added reusable import progress panel.
- Added categorized mapping panel with search, filters, mapped counts, and required unmapped counts.
- Added auto-map suggestions using normalized alias matching.
- Added template save/load placeholder buttons.
- Updated import pages to show progress and safer validation summaries.

## Safety Notes

Imports remain local. This change does not upload files, use cloud OCR, call external parsing services, submit ACH, or submit tax filings.
