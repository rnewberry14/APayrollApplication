# ClearPath Payroll Feature Guide

## Overview
This document describes the payroll tax API abstraction, direct deposit models, and ACH payment service features implemented in the ClearPath Payroll application.

## 1. What the feature does

### Tax API abstraction
- Provides a structured interface for payroll tax calculation via `ITaxCalculationService`.
- Defines request and response models for employee taxes and employer taxes.
- Supports federal, state, local, Social Security, and Medicare placeholders.
- Includes support for residence and work address data, gross wages, taxable wages, pay period dates, pay date, filing status, and external API reference tracking.
- Uses `FakeTaxCalculationService` as a sandbox implementation for development and tests.

### Direct deposit models
- Adds secure direct deposit entities for employee bank accounts, company funding accounts, deposit batches, and batch items.
- Ensures raw bank account numbers are not stored; only token placeholders and last 4 digits are retained.
- Tracks batch lifecycle status from `Draft` through `Ready`, `Submitted`, `Settled`, `Failed`, and `Reversed`.
- Tracks individual item status and return/rejection details.

### ACH payment service
- Provides a structured interface for ACH batch submission via `IAchPaymentService`.
- Defines request and response models for ACH batch submission, status tracking, and cancellation.
- Supports tokenized routing numbers and account numbers without storing raw banking data.
- Includes ACH return code support (R01 through R10) for handling rejected or returned payments.
- Uses `FakeAchPaymentService` as a sandbox implementation that simulates batch submissions, return codes, and settlement without processing real payments.
- Operates in sandbox mode by default to prevent accidental real transfers during development and testing.

## 2. Who can use it
- Payroll developers can use this feature to integrate tax calculation and direct deposit functionality into the payroll processing workflow.
- Payroll operations teams can review and manage direct deposit batches and employee payment items.
- Support staff can use the documentation and error guidance to troubleshoot batch and tax issues.

## 3. Required permissions
- These features are intended for roles with payroll administration or system administration permissions.
- Direct deposit setup and batch submission should be restricted to authorized payroll admins.
- Access should be limited to users who can safely view payroll run data, employee bank account references, and ACH submission states.

## 4. Step-by-step instructions

### Tax API abstraction usage
1. Construct a `TaxCalculationRequest` with:
   - `CompanyId`
   - `PayPeriodStart`, `PayPeriodEnd`, `PayDate`
   - `PayFrequency`
   - `EmployeeTaxRequest` containing gross wages, taxable wages, filing status, withholding allowances, residence address, and work address.
2. Call `ITaxCalculationService.CalculateTaxesAsync(request)`.
3. Inspect the returned `TaxCalculationResponse`:
   - `EmployeeTaxLines` for federal, state, local, Social Security, and Medicare tax amounts
   - `EmployerTaxLines` for employer contributions
   - `ExternalApiReference` for audit and reconciliation
4. Use convenience totals in the response such as `TotalEmployeeTaxes` and `TotalEmployerTaxes`.

### Direct deposit model usage
1. Create or load an `EmployeeBankAccount` for the employee.
   - Store only tokenized routing/account placeholders.
   - Store `Last4` digits for display.
2. Create a `CompanyFundingAccount` for the company funding source.
3. Create a `DirectDepositBatch` and assign it to the correct `PayrollRunId`, `CompanyId`, and `CompanyFundingAccountId`.
4. Add `DirectDepositItem` entries to the batch for each employee payment.
5. Calculate `DirectDepositBatch.TotalAmount` from item amounts before submission.
6. Update the batch status through the lifecycle: `Draft` → `Ready` → `Submitted` → `Settled` or `Failed`.

### ACH payment service usage
1. Construct an `AchBatchRequest` with:
   - `BatchId`, `CompanyId`, `CompanyName`, `CompanyEin`
   - `FundingRoutingNumberToken`, `FundingAccountNumberToken` (tokenized, not raw data)
   - `EffectiveDate` and `SettlementDate`
   - `SandboxMode = true` (always use true for development; never set false unless production-approved)
   - `Items` list of `AchPaymentItem` entries containing:
     - `EmployeeId`, `Amount`
     - `RoutingNumberToken`, `AccountNumberToken` (tokenized)
     - `Last4` digits and account type for display
     - `IndividualName`
2. Call `IAchPaymentService.SubmitBatchAsync(request)` to submit the batch.
   - Returns `AchBatchResponse` with `BatchReference`, `ResponseTimestamp`, and item responses.
   - Do not submit real payments; this is a sandbox service only.
3. Call `IAchPaymentService.GetBatchStatusAsync(batchReference)` to check batch status.
   - Returns updated status and item settlement details.
4. Call `IAchPaymentService.CancelBatchAsync(batchReference)` to cancel a pending batch.
   - Marks all items as rejected with return code "X01".

## 5. Payroll, tax, ACH, or security warnings
- Do not use this fake tax service for production tax compliance; it is a placeholder.
- Tax amounts are illustrative and use simplified placeholder rates.
- **IMPORTANT:** The `FakeAchPaymentService` does not submit real ACH transfers. This is intentional and required for development and testing.
- Always set `SandboxMode = true` in `AchBatchRequest`. Do not set `SandboxMode = false` unless you have explicit approval to process real payments.
- ACH batches should only be submitted after the payroll run is approved and validated.
- Never store or log raw routing numbers, full bank account numbers, SSNs, or API secrets.
- Use token placeholders instead of raw ACH banking data.
- Review returned item codes and descriptions carefully; bad items may indicate bank account, routing, or account verification issues.
- The fake ACH service simulates a 20% failure rate to test return code handling. In production, monitor actual return codes and customer communications.

## 6. Common errors
- `ExternalItemReference` missing or invalid: batch tracking may fail.
- `ReturnCode` / `ReturnDescription` populated on `DirectDepositItem`: indicates rejected or returned payments.
- `TotalAmount` mismatch: batch total must equal sum of item amounts.
- Missing `ResidenceAddress` or `WorkAddress`: state/local tax calculations may not behave as expected.
- Incorrect `Status` transitions: only `Draft` batches should be edited.
- **ACH Return Codes:**
  - `R01`: Invalid account number
  - `R02`: Account closed
  - `R03`: No account/unable to locate
  - `R04`: Invalid routing number
  - `R05`: Debit authorized by customer account holder has been revoked
  - `R06`: Returned per customer request
  - `R07`: Settlement error
  - `R08`: Routing number check digit error
  - `R09`: Incorrect individual identification number
  - `R10`: Unable to process
  - `X01`: Batch cancelled (cancellation-specific code)
- `AchBatchResponse.IsSuccessful = false`: batch submission failed; check `ErrorMessage` for details.
- `AchPaymentItemResponse.Status = Returned` or `Rejected`: individual item failed; check item-level `ReturnCode` and `Description`.

## 7. Troubleshooting steps
- Verify `DirectDepositBatch.Status` matches the expected workflow.
- Confirm `Batch.TotalAmount` equals the sum of `DirectDepositItem.Amount` values.
- For returned items, inspect `ReturnCode` and `ReturnDescription` and correct the bank account tokens or employee account setup.
- For tax issues, verify the input `TaxCalculationRequest` fields: `GrossWages`, `TaxableWages`, `FilingStatus`, and address data.
- Use `ExternalApiReference` to track fake or future vendor responses.
- If direct deposit items are rejected, check whether the employee bank account is active and verified.
- **For ACH batch issues:**
  - Verify all `AchPaymentItem` amounts sum to `AchBatchRequest.TotalAmount`.
  - Confirm `EffectiveDate` and `SettlementDate` are valid and in the future.
  - Check `SandboxMode` is set to `true` in development/testing.
  - Inspect `AchBatchResponse.ItemResponses` for individual item status and return codes.
  - If batch reference is not found, verify the reference string matches exactly.
  - Use `GetBatchStatusAsync` to poll batch settlement progress (fake service settles immediately for items without return codes).
  - For cancelled batches, check `CancelBatchAsync` result; all items should have status `Rejected` and code `X01`.

## 8. What support should never ask the customer for
- Full bank account numbers
- Full routing numbers
- SSN in full or raw sensitive payroll data
- API keys, ACH processor credentials, or payroll vendor secrets
- Passwords or two-factor authentication codes

## 9. Technical files changed
- `src/ClearPathPayroll/Integrations/ITaxCalculationService.cs`
- `src/ClearPathPayroll/Integrations/TaxCalculationRequest.cs`
- `src/ClearPathPayroll/Integrations/TaxCalculationResponse.cs`
- `src/ClearPathPayroll/Integrations/FakeTaxCalculationService.cs`
- `src/ClearPathPayroll/Integrations/AchModels.cs` (AchPaymentStatus enum, AchBatchRequest, AchBatchResponse, AchPaymentItem, AchPaymentItemResponse)
- `src/ClearPathPayroll/Integrations/IAchPaymentService.cs` (SubmitBatchAsync, GetBatchStatusAsync, CancelBatchAsync)
- `src/ClearPathPayroll/Integrations/FakeAchPaymentService.cs` (sandbox implementation with 20% failure simulation)
- `src/ClearPathPayroll/Domain/EmployeeBankAccount.cs`
- `src/ClearPathPayroll/Domain/CompanyFundingAccount.cs`
- `src/ClearPathPayroll/Domain/DirectDepositBatch.cs`
- `src/ClearPathPayroll/Domain/DirectDepositItem.cs`
- `src/ClearPathPayroll/Data/PayrollDbContext.cs` (DbSet additions for new entities)
- `src/ClearPathPayroll/Program.cs` (dependency injection registration: ITaxCalculationService, IAchPaymentService)

## 10. Tests added
- `tests/ClearPathPayroll.Tests/TaxCalculationServiceTests.cs` (27 tests)
- `tests/ClearPathPayroll.Tests/DirectDepositModelsTests.cs` (4 tests)
- `tests/ClearPathPayroll.Tests/AchPaymentServiceTests.cs` (23 tests covering batch submission, status retrieval, cancellation, return codes, and edge cases)

**Test Coverage Summary:**
- Batch submission: successful submission, reference generation, item processing, total amount calculation, sandbox mode flag
- Item status tracking: trace number generation, status validation, return code assignment and descriptions
- Batch retrieval: existing batch lookup, not-found error handling, settlement simulation
- Batch cancellation: successful cancellation, all items marked as rejected with "X01" code
- Aggregate calculations: successful/failed item counts
- ACH return codes: validation of R01-R10 codes and descriptions
- Edge cases: empty batches, single items, large amounts
- Total: 114 tests passing (91 existing + 23 new ACH tests)

## Notes
- The tax API abstraction is intentionally designed to allow future vendor integration. The current implementation uses `FakeTaxCalculationService` for development.
- Direct deposit models are storage-only at this stage; submission logic is provided via `IAchPaymentService`.
- The ACH payment service is a sandbox-only implementation. The `FakeAchPaymentService` simulates ACH processing, including a 20% failure rate to test error handling and return codes. Do not use this service for real payments.
- All banking data is tokenized; no raw routing numbers, account numbers, or SSNs are stored in the models or transmitted in requests.
- Batch references are generated in the format `FAKE-ACH-{CompanyId}-{timestamp}-{random}` for sandbox tracking and audit trails.
