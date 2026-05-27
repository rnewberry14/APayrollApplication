# Official Source Excerpts - Admin Guide

## What This Feature Does

Official Source Excerpts gives administrators a local review workflow for official source excerpt records. It displays warning messages when a source is not marked current or when source dates are older than 12 months.

## Who Can Use It

Administrators responsible for maintaining local official source records.

## Required Permissions

- Official source record read access.
- Official source review log create access.

## Admin Steps

1. Confirm each record includes source name, title, available date, excerpt text, and citation link.
2. Open `/official-sources`.
3. Review warnings shown on each record.
4. Select **Mark reviewed** to create an `OfficialSourceReviewLog` entry.
5. Confirm the last reviewed date appears.

## Common Errors

- Missing citation link: add the official source link before using the excerpt in a workflow.
- Not-current warning: the record has `IsCurrent = false`.
- Review warning: retrieved date or revision date is older than 12 months.

## Cautions

- Do not use review logging as a currentness certification.
- Do not interpret excerpts as advice.
- ClearPath Payroll does not guarantee compliance.
