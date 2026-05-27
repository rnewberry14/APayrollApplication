# Help Center Technical Notes

## Route

- `/help`

## Implementation

- Page: `src/ClearPathPayroll/Components/Pages/HelpCenter.razor`
- Source articles: local markdown files under `docs/help/articles` and `docs/testing`
- Metadata: static local list in the page

The Help Center reads article markdown from `IWebHostEnvironment.ContentRootPath`, so packaged tester docs remain local to the extracted application folder.

## Boundaries

- No external documentation hosting.
- No telemetry.
- No usage tracking.
- No external calls.
- No advice wording.
