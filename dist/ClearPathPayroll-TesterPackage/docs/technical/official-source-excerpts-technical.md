# Official Source Excerpts - Technical Notes

## Overview

Official source review tracking is implemented with `OfficialSourceDocument`, `OfficialSourceReviewLog`, `OfficialSourceService`, and the `/official-sources` page.

## Data Model

- `OfficialSourceDocument`
  - Source name
  - Form/publication/instruction title
  - Revision date
  - Effective date
  - Retrieved date
  - Citation or official source link
  - Official excerpt text
  - `IsCurrent`
- `OfficialSourceReviewLog`
  - Source document ID
  - Reviewed date
  - Reviewed by user ID
  - Note

## Behavior

- `IsCurrent = false` displays: "This source has not been marked current. Verify the official source before relying on it."
- Retrieved date or revision date older than 12 months displays: "This source may need review. Verify the current official source."
- **Mark reviewed** creates a review log entry.
- Review logging leaves `IsCurrent` unchanged.

## Tests

- `OfficialSourceServiceTests`
- `OfficialSourcesPageTests`

## Cautions

- Do not add automatic source certification logic.
- Do not summarize official excerpts into advice.
- Do not guarantee compliance.
