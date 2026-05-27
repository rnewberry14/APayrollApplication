# Official Source Excerpts - Troubleshooting

## What This Feature Does

The feature displays official source excerpt records and creates review log entries when users select **Mark reviewed**.

## Who Can Use It

Support users and administrators with access to official source records.

## Required Permissions

- Read official source records.
- Create official source review logs.

## Common Errors

| Symptom | Likely Cause | Troubleshooting Step |
| --- | --- | --- |
| Not-current warning appears | `IsCurrent` is false | Verify the official source record before relying on it |
| Review warning appears | Retrieved or revision date is older than 12 months | Verify the current official source |
| Last reviewed is blank | No review log exists | Select **Mark reviewed** after review |
| Review cannot be recorded | The database save failed | Check application logs and database connectivity |

## Security And Source Cautions

- Do not add unofficial text as an official excerpt.
- Do not edit official excerpt text into guidance or recommendations.
- User is responsible for verifying applicability.
- ClearPath Payroll does not provide legal, tax, financial, accounting, payroll compliance, or licensed professional advice.
