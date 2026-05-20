namespace ClearPathPayroll.Integrations;

/// <summary>
/// Fake ACH payment service for development and testing.
/// Simulates ACH batch submission without making real transfers.
/// All operations are in sandbox mode.
/// </summary>
public class FakeAchPaymentService : IAchPaymentService
{
    private readonly Dictionary<string, AchBatchResponse> _batches = new();
    private readonly Random _random = new();

    /// <summary>
    /// ACH return codes for realistic rejection simulation.
    /// </summary>
    private static readonly string[] AchReturnCodes = new[]
    {
        "R01", // Invalid account number
        "R02", // Account closed
        "R03", // No account/unable to locate
        "R04", // Invalid routing number
        "R05", // Unauthorized user-initiated debit
        "R06", // Returned per customer request
        "R07", // Authorization revoked
        "R08", // Payment stopped
        "R09", // Uncollected funds
        "R10", // Customer advises not authorized
    };

    /// <summary>
    /// Submits an ACH batch (sandbox mode only - no real transfers).
    /// </summary>
    public Task<AchBatchResponse> SubmitBatchAsync(AchBatchRequest request)
    {
        var response = new AchBatchResponse
        {
            IsSuccessful = true,
            ResponseTimestamp = DateTime.UtcNow,
            BatchReference = GenerateBatchReference(request),
            SubmissionTimestamp = DateTime.UtcNow,
            SettlementDate = request.SettlementDate,
            SandboxMode = request.SandboxMode,
            ApiVersion = request.ApiVersion
        };

        // Process each item
        foreach (var item in request.Items)
        {
            var itemResponse = ProcessPaymentItem(item);
            response.ItemResponses.Add(itemResponse);
        }

        // Store batch for later retrieval
        if (response.BatchReference != null)
        {
            _batches[response.BatchReference] = response;
        }

        return Task.FromResult(response);
    }

    /// <summary>
    /// Retrieves the status of a previously submitted batch.
    /// </summary>
    public Task<AchBatchResponse> GetBatchStatusAsync(string batchReference)
    {
        if (_batches.TryGetValue(batchReference, out var batch))
        {
            // Simulate processing - update some items to Settled status
            var updatedResponse = new AchBatchResponse
            {
                IsSuccessful = batch.IsSuccessful,
                ResponseTimestamp = DateTime.UtcNow,
                BatchReference = batch.BatchReference,
                SubmissionTimestamp = batch.SubmissionTimestamp,
                SettlementDate = batch.SettlementDate,
                SandboxMode = batch.SandboxMode,
                ApiVersion = batch.ApiVersion
            };

            // Simulate settlement progress
            foreach (var item in batch.ItemResponses)
            {
                var updated = new AchPaymentItemResponse
                {
                    EmployeeId = item.EmployeeId,
                    Amount = item.Amount,
                    TraceNumber = item.TraceNumber,
                    ReturnCode = item.ReturnCode,
                    Description = item.Description,
                    Status = item.ReturnCode == null 
                        ? AchPaymentStatus.Settled 
                        : item.Status
                };
                updatedResponse.ItemResponses.Add(updated);
            }

            return Task.FromResult(updatedResponse);
        }

        // Batch not found
        var errorResponse = new AchBatchResponse
        {
            IsSuccessful = false,
            ResponseTimestamp = DateTime.UtcNow,
            ErrorMessage = $"Batch reference '{batchReference}' not found.",
            SandboxMode = true
        };

        return Task.FromResult(errorResponse);
    }

    /// <summary>
    /// Cancels a pending ACH batch.
    /// </summary>
    public Task<AchBatchResponse> CancelBatchAsync(string batchReference)
    {
        if (_batches.TryGetValue(batchReference, out var batch))
        {
            // Mark batch as cancelled by returning rejected status for all items
            var cancelledResponse = new AchBatchResponse
            {
                IsSuccessful = true,
                ResponseTimestamp = DateTime.UtcNow,
                BatchReference = batchReference,
                SubmissionTimestamp = batch.SubmissionTimestamp,
                SettlementDate = batch.SettlementDate,
                SandboxMode = batch.SandboxMode,
                ApiVersion = batch.ApiVersion,
                ErrorMessage = "Batch cancelled by request."
            };

            foreach (var item in batch.ItemResponses)
            {
                cancelledResponse.ItemResponses.Add(new AchPaymentItemResponse
                {
                    EmployeeId = item.EmployeeId,
                    Amount = item.Amount,
                    Status = AchPaymentStatus.Rejected,
                    ReturnCode = "X01",
                    Description = "Cancelled by request"
                });
            }

            // Update stored batch
            _batches[batchReference] = cancelledResponse;

            return Task.FromResult(cancelledResponse);
        }

        var notFoundResponse = new AchBatchResponse
        {
            IsSuccessful = false,
            ResponseTimestamp = DateTime.UtcNow,
            ErrorMessage = $"Batch reference '{batchReference}' not found.",
            SandboxMode = true
        };

        return Task.FromResult(notFoundResponse);
    }

    /// <summary>
    /// Processes an individual payment item, simulating returns/rejections.
    /// </summary>
    private AchPaymentItemResponse ProcessPaymentItem(AchPaymentItem item)
    {
        var response = new AchPaymentItemResponse
        {
            EmployeeId = item.EmployeeId,
            Amount = item.Amount,
            TraceNumber = GenerateTraceNumber(item),
            Status = AchPaymentStatus.Transmitted
        };

        // Simulate occasional failures (20% chance of return/rejection)
        if (_random.Next(100) < 20)
        {
            var returnCode = AchReturnCodes[_random.Next(AchReturnCodes.Length)];
            response.Status = returnCode.StartsWith("R") 
                ? AchPaymentStatus.Returned 
                : AchPaymentStatus.Rejected;
            response.ReturnCode = returnCode;
            response.Description = GetReturnCodeDescription(returnCode);
        }
        else
        {
            response.Status = AchPaymentStatus.Settled;
        }

        return response;
    }

    /// <summary>
    /// Generates a fake batch reference for external tracking.
    /// </summary>
    private string GenerateBatchReference(AchBatchRequest request)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"FAKE-ACH-{request.CompanyId}-{timestamp}-{_random.Next(10000):D5}";
    }

    /// <summary>
    /// Generates a fake trace number for payment tracking.
    /// </summary>
    private string GenerateTraceNumber(AchPaymentItem item)
    {
        return $"TRACE-{item.EmployeeId}-{DateTime.UtcNow.Ticks % 1000000:D6}";
    }

    /// <summary>
    /// Returns a description for a given ACH return code.
    /// </summary>
    private string GetReturnCodeDescription(string returnCode)
    {
        return returnCode switch
        {
            "R01" => "Invalid account number",
            "R02" => "Account closed",
            "R03" => "No account/unable to locate",
            "R04" => "Invalid routing number",
            "R05" => "Unauthorized user-initiated debit",
            "R06" => "Returned per customer request",
            "R07" => "Authorization revoked",
            "R08" => "Payment stopped",
            "R09" => "Uncollected funds",
            "R10" => "Customer advises not authorized",
            "X01" => "Cancelled by request",
            _ => "Unknown return code"
        };
    }
}
