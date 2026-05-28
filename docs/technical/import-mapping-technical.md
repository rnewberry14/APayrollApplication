# Import Mapping Technical Notes

## Components

- `ImportProgress` displays import stage, status, row count, warning count, and error count.
- `ImportMappingPanel` displays categorized card-based mapping, source/target filters, required/unmapped counts, and template placeholder actions.

## Services

- `ImportProgressModel` tracks progress stages.
- `ImportFieldAliasMatcher` normalizes aliases by ignoring spaces, underscores, dashes, punctuation, and case.

## Safety

Import errors shown in the UI use safe summaries. Original files are read from the browser upload stream and are not uploaded to external services by this workflow.

## Current Limitations

Mapping template save/load buttons are placeholders for tester UX review. They do not persist templates yet.
