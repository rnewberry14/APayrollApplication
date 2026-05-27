using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Handles preparing and submitting direct deposit batches for payroll runs.
/// </summary>
public class DirectDepositSubmissionService
{
    private readonly PayrollDbContext _context;
    private readonly IAchPaymentService _achService;
    private readonly PayrollService _payrollService;

    public DirectDepositSubmissionService(PayrollDbContext context, IAchPaymentService achService, PayrollService payrollService)
    {
        _context = context;
        _achService = achService;
        _payrollService = payrollService;
    }

    public async Task<DirectDepositBatch?> GetLatestBatchForPayrollRunAsync(int payrollRunId)
    {
        return await _context.DirectDepositBatches
            .Include(b => b.Items)
            .Where(b => b.PayrollRunId == payrollRunId)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<DirectDepositBatch> SubmitFakePayrollRunDirectDepositAsync(int payrollRunId, string performedByUserId)
    {
        if (_achService is not FakeAchPaymentService)
        {
            throw new InvalidOperationException("Fake direct deposit submission requires FakeAchPaymentService. Production ACH submission is not allowed from this workflow.");
        }

        var payrollRun = await _context.PayrollRuns.FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);
        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");
        }

        var fundingAccount = await _context.CompanyFundingAccounts
            .Where(f => f.CompanyId == payrollRun.CompanyId && f.IsActive)
            .OrderByDescending(f => f.IsPrimary)
            .ThenBy(f => f.CompanyFundingAccountId)
            .FirstOrDefaultAsync();

        if (fundingAccount == null)
        {
            throw new InvalidOperationException("No active company funding account was found for fake direct deposit.");
        }

        return await SubmitPayrollRunDirectDepositAsync(
            payrollRunId,
            fundingAccount.CompanyFundingAccountId,
            performedByUserId,
            sandboxMode: true);
    }

    /// <summary>
    /// Submits direct deposit batch for an approved payroll run.
    /// </summary>
    public async Task<DirectDepositBatch> SubmitPayrollRunDirectDepositAsync(int payrollRunId, int companyFundingAccountId, string performedByUserId, bool sandboxMode = true)
    {
        // Load payroll run and necessary navigation properties
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.Employee)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.NetPayLines)
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");

        if (payrollRun.Status != PayrollStatus.Approved)
            throw new InvalidOperationException($"Only Approved payroll runs can be submitted. Current status: {payrollRun.Status}.");

        // Prevent duplicate submission: existing submitted or settled batches
        var existing = await _context.DirectDepositBatches
            .Where(b => b.PayrollRunId == payrollRunId && (b.Status == DirectDepositBatchStatus.Submitted || b.Status == DirectDepositBatchStatus.Settled))
            .FirstOrDefaultAsync();
        if (existing != null)
            throw new InvalidOperationException($"Payroll run {payrollRunId} already has a submitted/settled direct deposit batch (BatchId={existing.BatchId}).");

        // Fetch funding account
        var fundingAccount = await _context.CompanyFundingAccounts.FirstOrDefaultAsync(f => f.CompanyFundingAccountId == companyFundingAccountId && f.CompanyId == payrollRun.CompanyId && f.IsActive);
        if (fundingAccount == null)
            throw new InvalidOperationException($"Funding account {companyFundingAccountId} not found or not active for company {payrollRun.CompanyId}.");

        // Build list of direct deposit items from payroll run net pay lines
        var items = new List<(PayrollRunEmployee Pre, NetPayLine NetPay, EmployeeBankAccount Bank)>();

        foreach (var pre in payrollRun.PayrollRunEmployees)
        {
            foreach (var net in pre.NetPayLines.Where(n => n.PaymentMethod == "DirectDeposit"))
            {
                if (net.Amount <= 0) continue;

                // Get verified active bank account for employee
                var bank = await _context.EmployeeBankAccounts
                    .Where(b => b.EmployeeId == pre.EmployeeId && b.IsActive && b.VerificationStatus == VerificationStatus.Verified)
                    .OrderByDescending(b => b.IsActive)
                    .FirstOrDefaultAsync();

                if (bank == null)
                    throw new InvalidOperationException($"Employee {pre.EmployeeId} missing verified bank account for direct deposit.");

                items.Add((pre, net, bank));
            }
        }

        if (!items.Any())
            throw new InvalidOperationException($"No direct deposit items found for payroll run {payrollRunId}.");

        // Create DirectDepositBatch
        var batch = new DirectDepositBatch
        {
            PayrollRunId = payrollRunId,
            CompanyId = payrollRun.CompanyId,
            CompanyFundingAccountId = companyFundingAccountId,
            PayDate = payrollRun.PayDate,
            Status = DirectDepositBatchStatus.Draft,
            Description = $"Payroll {payrollRun.PayPeriodStart:yyyy-MM-dd} to {payrollRun.PayPeriodEnd:yyyy-MM-dd}",
            CreatedByUserId = performedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.DirectDepositBatches.Add(batch);
        await _context.SaveChangesAsync(); // get BatchId

        // Add items
        foreach (var (pre, net, bank) in items)
        {
            var ddi = new DirectDepositItem
            {
                BatchId = batch.BatchId,
                EmployeeId = pre.EmployeeId,
                EmployeeBankAccountId = bank.EmployeeBankAccountId,
                Amount = net.Amount,
                Status = DirectDepositItemStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _context.DirectDepositItems.Add(ddi);
        }

        await _context.SaveChangesAsync();

        // Prepare ACH request
        var achRequest = new AchBatchRequest
        {
            BatchId = batch.BatchId,
            CompanyId = batch.CompanyId,
            CompanyName = payrollRun.Company?.LegalName ?? string.Empty,
            CompanyEin = payrollRun.Company?.FEIN ?? string.Empty,
            FundingRoutingNumberToken = fundingAccount.RoutingNumberToken,
            FundingAccountNumberToken = fundingAccount.AccountNumberToken,
            EffectiveDate = batch.PayDate,
            SettlementDate = batch.PayDate,
            SandboxMode = sandboxMode,
            ApiVersion = "1.0"
        };

        // Populate items from DB to ensure correct IDs
        var dbItems = await _context.DirectDepositItems
            .Include(i => i.BankAccount)
            .Where(i => i.BatchId == batch.BatchId)
            .ToListAsync();

        foreach (var dbItem in dbItems)
        {
            achRequest.Items.Add(new AchPaymentItem
            {
                EmployeeId = dbItem.EmployeeId,
                Amount = dbItem.Amount,
                RoutingNumberToken = dbItem.BankAccount?.RoutingNumberToken ?? string.Empty,
                AccountNumberToken = dbItem.BankAccount?.AccountNumberToken ?? string.Empty,
                Last4 = dbItem.BankAccount?.Last4 ?? string.Empty,
                AccountType = dbItem.BankAccount?.AccountType.ToString() ?? "Checking",
                IndividualName = (dbItem.BankAccount?.Employee?.FirstName ?? string.Empty) + " " + (dbItem.BankAccount?.Employee?.LastName ?? string.Empty)
            });
        }

        AchBatchResponse response;
        try
        {
            response = await _achService.SubmitBatchAsync(achRequest);
        }
        catch (Exception ex)
        {
            // Mark batch failed
            batch.Status = DirectDepositBatchStatus.Failed;
            batch.ErrorMessage = ex.Message;
            batch.SubmittedByUserId = performedByUserId;
            await _context.SaveChangesAsync();

            _context.AuditLogEntries.Add(new AuditLogEntry
            {
                PayrollRunId = payrollRunId,
                EventType = "DirectDepositSubmissionFailed",
                Description = $"Direct deposit submission for payroll run {payrollRunId} failed: {ex.Message}",
                CreatedByUserId = performedByUserId,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return batch;
        }

        // Process response
        if (!response.IsSuccessful)
        {
            batch.Status = DirectDepositBatchStatus.Failed;
            batch.ErrorMessage = response.ErrorMessage;
            batch.SubmittedByUserId = performedByUserId;
            batch.SubmittedAt = response.ResponseTimestamp;
            await _context.SaveChangesAsync();

            _context.AuditLogEntries.Add(new AuditLogEntry
            {
                PayrollRunId = payrollRunId,
                EventType = "DirectDepositSubmissionFailed",
                Description = $"Direct deposit submission for payroll run {payrollRunId} returned failure: {response.ErrorMessage}",
                CreatedByUserId = performedByUserId,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return batch;
        }

        // Success: update batch and items
        batch.ExternalBatchReference = response.BatchReference;
        batch.Status = DirectDepositBatchStatus.Submitted;
        batch.SubmittedAt = response.SubmissionTimestamp ?? DateTime.UtcNow;
        batch.SubmittedByUserId = performedByUserId;
        batch.TotalAmount = response.TotalAmount;

        // Map item responses
        foreach (var itemResp in response.ItemResponses)
        {
            var dbItem = await _context.DirectDepositItems.FirstOrDefaultAsync(i => i.BatchId == batch.BatchId && i.EmployeeId == itemResp.EmployeeId && i.Amount == itemResp.Amount);
            if (dbItem == null) continue;

            dbItem.ExternalItemReference = itemResp.TraceNumber;
            dbItem.ReturnCode = itemResp.ReturnCode;
            dbItem.ReturnDescription = itemResp.Description;

            if (itemResp.Status == AchPaymentStatus.Settled)
            {
                dbItem.Status = DirectDepositItemStatus.Settled;
                dbItem.SettledAt = DateTime.UtcNow;
            }
            else if (itemResp.Status == AchPaymentStatus.Rejected)
            {
                dbItem.Status = DirectDepositItemStatus.Rejected;
            }
            else if (itemResp.Status == AchPaymentStatus.Returned)
            {
                dbItem.Status = DirectDepositItemStatus.Returned;
            }
            else
            {
                dbItem.Status = DirectDepositItemStatus.Transmitted;
                dbItem.TransmittedAt = DateTime.UtcNow;
            }
        }

        // Persist changes
        await _context.SaveChangesAsync();

        // Update payroll run status to Submitted
        await _payrollService.UpdatePayrollStatusAsync(payrollRunId, PayrollStatus.Submitted, performedByUserId);

        _context.AuditLogEntries.Add(new AuditLogEntry
        {
            PayrollRunId = payrollRunId,
            EventType = "DirectDepositSubmitted",
            Description = $"Direct deposit batch {batch.BatchId} submitted for payroll run {payrollRunId}. ExternalRef={batch.ExternalBatchReference}",
            CreatedByUserId = performedByUserId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return batch;
    }
}
