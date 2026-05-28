# Import Mapping Administration

Import mapping is local-first. ClearPath Payroll does not upload import files, call cloud parsing, or store original files permanently unless a future user-controlled local copy option is added.

## Administrative Scope

- Progress is shown with reusable `ImportProgress`.
- Mapping UX is shown with reusable `ImportMappingPanel`.
- Alias matching is handled by `ImportFieldAliasMatcher`.
- Template save/load buttons are placeholders for tester review.

## Operational Notes

- Auto-map suggestions do not save imports.
- Users must validate and confirm before data is saved.
- Required unmapped fields block validation from the mapping panel.
- Error summaries are field-based and should not include raw sensitive row values.

## Safety Notes

Do not use this workflow to transmit files externally. Do not provide advice about what data a user is required to import.
