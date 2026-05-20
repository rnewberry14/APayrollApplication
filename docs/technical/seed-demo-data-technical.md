# Seed Demo Data — Technical Notes

What it does

- `SeedDataService` creates a demo company, three employees (hourly, salary, direct deposit), a pay schedule, a draft payroll run, earning/deduction/tax/netpay lines, a demo employee bank account (tokenized), and a demo company funding account.

Implementation

- Service: `ClearPathPayroll.Services.SeedDataService` (registered DI as scoped in Program.cs).
- UI: `Components/Pages/SeedDemoData.razor` — a Blazor page available in Development or Local Prototype Mode.
- Protection: `PrototypeModeHelper.ShouldUseLocalPrototypeMode` prevents seeding in production.
- Tokens and last4 use obvious demo values (e.g., `demo-account-token-0000`, `Last4 = "0000"`).

Where to look in code

- Service: `src/ClearPathPayroll/Services/SeedDataService.cs`
- Page: `src/ClearPathPayroll/Components/Pages/SeedDemoData.razor`
- DI: `src/ClearPathPayroll/Program.cs`

Reset/clear instructions

- Use SQL Server Object Explorer or `sqllocaldb` to locate and delete the demo database.
- Or remove rows where `Company.LegalName = 'Demo Company (Local Prototype)'`.

Security

- No real SSNs, routing numbers, or account numbers are stored.
- Demo tokens are placeholders and marked clearly.
