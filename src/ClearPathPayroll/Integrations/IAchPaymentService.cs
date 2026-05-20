namespace ClearPathPayroll.Integrations;

/// <summary>
/// Interface for ACH payment service.
/// Supports submission, status tracking, and cancellation of ACH batches.
/// </summary>
public interface IAchPaymentService
{
    /// <summary>
    /// Submits an ACH batch for processing.
    /// In sandbox mode, does not create real ACH transfers.
    /// </summary>
    /// <param name="request">ACH batch request with payment items.</param>
    /// <returns>Batch submission response with item statuses and external references.</returns>
    Task<AchBatchResponse> SubmitBatchAsync(AchBatchRequest request);

    /// <summary>
    /// Retrieves the current status of a previously submitted batch.
    /// </summary>
    /// <param name="batchReference">External batch reference from initial submission.</param>
    /// <returns>Current batch status and item statuses.</returns>
    Task<AchBatchResponse> GetBatchStatusAsync(string batchReference);

    /// <summary>
    /// Cancels a pending ACH batch before settlement.
    /// </summary>
    /// <param name="batchReference">External batch reference to cancel.</param>
    /// <returns>Cancellation response with updated status.</returns>
    Task<AchBatchResponse> CancelBatchAsync(string batchReference);
}
