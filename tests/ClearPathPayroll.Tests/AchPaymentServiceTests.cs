using ClearPathPayroll.Integrations;
using Xunit;

namespace ClearPathPayroll.Tests;

/// <summary>
/// Unit tests for the ACH payment service abstraction.
/// </summary>
public class AchPaymentServiceTests
{
    private readonly FakeAchPaymentService _service = new();

    #region Batch Submission Tests

    [Fact]
    public async Task SubmitBatch_ReturnsSuccessfulResponse()
    {
        var request = CreateBasicBatchRequest();

        var response = await _service.SubmitBatchAsync(request);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessful);
        Assert.NotNull(response.BatchReference);
        Assert.NotEmpty(response.BatchReference);
        Assert.True(response.SandboxMode);
    }

    [Fact]
    public async Task SubmitBatch_GeneratesExternalBatchReference()
    {
        var request = CreateBasicBatchRequest();

        var response = await _service.SubmitBatchAsync(request);

        Assert.NotNull(response.BatchReference);
        Assert.Contains("FAKE-ACH", response.BatchReference);
        Assert.Contains(request.CompanyId.ToString(), response.BatchReference);
    }

    [Fact]
    public async Task SubmitBatch_ProcessesAllItems()
    {
        var request = new AchBatchRequest
        {
            BatchId = 1,
            CompanyId = 1,
            CompanyName = "Test Company",
            CompanyEin = "12-3456789",
            FundingRoutingNumberToken = "ROUTING-123",
            FundingAccountNumberToken = "ACCOUNT-456",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(1),
            SettlementDate = DateTime.UtcNow.Date.AddDays(2),
            SandboxMode = true,
            Items = new List<AchPaymentItem>
            {
                new()
                {
                    EmployeeId = 1,
                    Amount = 1000m,
                    RoutingNumberToken = "ROUTING-001",
                    AccountNumberToken = "ACCOUNT-001",
                    Last4 = "1234",
                    IndividualName = "John Doe"
                },
                new()
                {
                    EmployeeId = 2,
                    Amount = 1500m,
                    RoutingNumberToken = "ROUTING-002",
                    AccountNumberToken = "ACCOUNT-002",
                    Last4 = "5678",
                    IndividualName = "Jane Smith"
                },
                new()
                {
                    EmployeeId = 3,
                    Amount = 2000m,
                    RoutingNumberToken = "ROUTING-003",
                    AccountNumberToken = "ACCOUNT-003",
                    Last4 = "9012",
                    IndividualName = "Bob Johnson"
                }
            }
        };

        var response = await _service.SubmitBatchAsync(request);

        Assert.Equal(3, response.ItemResponses.Count);
        Assert.All(response.ItemResponses, item => Assert.NotEqual(AchPaymentStatus.Pending, item.Status));
    }

    [Fact]
    public async Task SubmitBatch_CalculatesTotalAmount()
    {
        var request = CreateBasicBatchRequest();
        var expectedTotal = request.Items.Sum(i => i.Amount);

        var response = await _service.SubmitBatchAsync(request);

        Assert.Equal(expectedTotal, response.TotalAmount);
    }

    [Fact]
    public async Task SubmitBatch_SetsSandboxModeFromRequest()
    {
        var request = CreateBasicBatchRequest();
        request.SandboxMode = true;

        var response = await _service.SubmitBatchAsync(request);

        Assert.True(response.SandboxMode);
    }

    [Fact]
    public async Task SubmitBatch_PreservesApiVersion()
    {
        var request = CreateBasicBatchRequest();
        request.ApiVersion = "2.0";

        var response = await _service.SubmitBatchAsync(request);

        Assert.Equal("2.0", response.ApiVersion);
    }

    #endregion

    #region Item Status Tests

    [Fact]
    public async Task SubmitBatch_AssignsTraceNumberToEachItem()
    {
        var request = CreateBasicBatchRequest();

        var response = await _service.SubmitBatchAsync(request);

        Assert.All(response.ItemResponses, item =>
        {
            Assert.NotNull(item.TraceNumber);
            Assert.NotEmpty(item.TraceNumber);
            Assert.Contains("TRACE", item.TraceNumber);
        });
    }

    [Fact]
    public async Task SubmitBatch_ItemsHaveStatus()
    {
        var request = CreateBasicBatchRequest();

        var response = await _service.SubmitBatchAsync(request);

        Assert.All(response.ItemResponses, item =>
        {
            Assert.True(
                item.Status == AchPaymentStatus.Transmitted ||
                item.Status == AchPaymentStatus.Settled ||
                item.Status == AchPaymentStatus.Returned ||
                item.Status == AchPaymentStatus.Rejected,
                $"Status {item.Status} is not valid"
            );
        });
    }

    [Fact]
    public async Task SubmitBatch_ReturnedItemsHaveReturnCode()
    {
        var request = CreateBasicBatchRequest();

        // Submit multiple batches to potentially get some with return codes
        var responses = new List<AchBatchResponse>();
        for (int i = 0; i < 5; i++)
        {
            responses.Add(await _service.SubmitBatchAsync(request));
        }

        var returnedItems = responses
            .SelectMany(r => r.ItemResponses)
            .Where(i => i.Status == AchPaymentStatus.Returned);

        if (returnedItems.Any())
        {
            Assert.All(returnedItems, item =>
            {
                Assert.NotNull(item.ReturnCode);
                Assert.NotEmpty(item.ReturnCode);
                Assert.StartsWith("R", item.ReturnCode);
            });
        }
    }

    [Fact]
    public async Task SubmitBatch_ReturnedItemsHaveDescription()
    {
        var request = CreateBasicBatchRequest();

        var responses = new List<AchBatchResponse>();
        for (int i = 0; i < 5; i++)
        {
            responses.Add(await _service.SubmitBatchAsync(request));
        }

        var returnedItems = responses
            .SelectMany(r => r.ItemResponses)
            .Where(i => i.Status == AchPaymentStatus.Returned);

        if (returnedItems.Any())
        {
            Assert.All(returnedItems, item =>
            {
                Assert.NotNull(item.Description);
                Assert.NotEmpty(item.Description);
            });
        }
    }

    #endregion

    #region Batch Retrieval Tests

    [Fact]
    public async Task GetBatchStatus_ReturnsExistingBatch()
    {
        var submitRequest = CreateBasicBatchRequest();
        var submitResponse = await _service.SubmitBatchAsync(submitRequest);

        var statusResponse = await _service.GetBatchStatusAsync(submitResponse.BatchReference!);

        Assert.NotNull(statusResponse);
        Assert.True(statusResponse.IsSuccessful);
        Assert.Equal(submitResponse.BatchReference, statusResponse.BatchReference);
    }

    [Fact]
    public async Task GetBatchStatus_ReturnsNotFoundForInvalidReference()
    {
        var response = await _service.GetBatchStatusAsync("INVALID-REFERENCE");

        Assert.False(response.IsSuccessful);
        Assert.NotNull(response.ErrorMessage);
        Assert.Contains("not found", response.ErrorMessage);
    }

    [Fact]
    public async Task GetBatchStatus_SimulatesSettlement()
    {
        var submitRequest = CreateBasicBatchRequest();
        var submitResponse = await _service.SubmitBatchAsync(submitRequest);

        var statusResponse = await _service.GetBatchStatusAsync(submitResponse.BatchReference!);

        // Items without return codes should be marked as Settled
        var settledItems = statusResponse.ItemResponses
            .Where(i => i.ReturnCode == null)
            .ToList();

        Assert.All(settledItems, item =>
            Assert.Equal(AchPaymentStatus.Settled, item.Status)
        );
    }

    #endregion

    #region Batch Cancellation Tests

    [Fact]
    public async Task CancelBatch_SucceedsForValidReference()
    {
        var submitRequest = CreateBasicBatchRequest();
        var submitResponse = await _service.SubmitBatchAsync(submitRequest);

        var cancelResponse = await _service.CancelBatchAsync(submitResponse.BatchReference!);

        Assert.True(cancelResponse.IsSuccessful);
        Assert.Equal(submitResponse.BatchReference, cancelResponse.BatchReference);
    }

    [Fact]
    public async Task CancelBatch_ReturnsErrorForInvalidReference()
    {
        var response = await _service.CancelBatchAsync("INVALID-REFERENCE");

        Assert.False(response.IsSuccessful);
        Assert.NotNull(response.ErrorMessage);
    }

    [Fact]
    public async Task CancelBatch_MarksAllItemsAsRejected()
    {
        var submitRequest = CreateBasicBatchRequest();
        var submitResponse = await _service.SubmitBatchAsync(submitRequest);

        var cancelResponse = await _service.CancelBatchAsync(submitResponse.BatchReference!);

        Assert.All(cancelResponse.ItemResponses, item =>
        {
            Assert.Equal(AchPaymentStatus.Rejected, item.Status);
            Assert.Equal("X01", item.ReturnCode);
        });
    }

    #endregion

    #region Aggregate Calculation Tests

    [Fact]
    public async Task SubmitBatch_SuccessfulItemCountIsAccurate()
    {
        // Submit several batches and verify count
        var request = CreateBasicBatchRequest();
        var responses = new List<AchBatchResponse>();

        for (int i = 0; i < 3; i++)
        {
            responses.Add(await _service.SubmitBatchAsync(request));
        }

        Assert.All(responses, response =>
        {
            var actualSuccess = response.ItemResponses.Count(i => i.Status == AchPaymentStatus.Settled);
            Assert.Equal(actualSuccess, response.SuccessfulItemCount);
        });
    }

    [Fact]
    public async Task SubmitBatch_FailedItemCountIsAccurate()
    {
        var request = CreateBasicBatchRequest();
        var responses = new List<AchBatchResponse>();

        for (int i = 0; i < 3; i++)
        {
            responses.Add(await _service.SubmitBatchAsync(request));
        }

        Assert.All(responses, response =>
        {
            var actualFailed = response.ItemResponses.Count(i => 
                i.Status == AchPaymentStatus.Returned || 
                i.Status == AchPaymentStatus.Rejected
            );
            Assert.Equal(actualFailed, response.FailedItemCount);
        });
    }

    #endregion

    #region ACH Return Code Tests

    [Fact]
    public async Task SubmitBatch_ReturnCodesAreValid()
    {
        var validCodes = new[] { "R01", "R02", "R03", "R04", "R05", "R06", "R07", "R08", "R09", "R10" };
        var request = CreateBasicBatchRequest();

        var responses = new List<AchBatchResponse>();
        for (int i = 0; i < 10; i++)
        {
            responses.Add(await _service.SubmitBatchAsync(request));
        }

        var returnedItems = responses
            .SelectMany(r => r.ItemResponses)
            .Where(i => i.Status == AchPaymentStatus.Returned);

        Assert.All(returnedItems, item =>
        {
            Assert.Contains(item.ReturnCode, validCodes);
        });
    }

    [Fact]
    public async Task SubmitBatch_R01_InvalidAccountNumber()
    {
        var request = CreateBasicBatchRequest();

        // Submit many batches to increase chance of getting R01
        var responses = new List<AchBatchResponse>();
        for (int i = 0; i < 20; i++)
        {
            responses.Add(await _service.SubmitBatchAsync(request));
        }

        var r01Items = responses
            .SelectMany(r => r.ItemResponses)
            .Where(i => i.ReturnCode == "R01");

        if (r01Items.Any())
        {
            Assert.All(r01Items, item =>
            {
                Assert.Contains("Invalid account number", item.Description);
            });
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task SubmitBatch_WithEmptyItemList_ReturnsSuccessful()
    {
        var request = new AchBatchRequest
        {
            BatchId = 1,
            CompanyId = 1,
            CompanyName = "Test Company",
            CompanyEin = "12-3456789",
            FundingRoutingNumberToken = "ROUTING-123",
            FundingAccountNumberToken = "ACCOUNT-456",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(1),
            SettlementDate = DateTime.UtcNow.Date.AddDays(2),
            SandboxMode = true,
            Items = new List<AchPaymentItem>()
        };

        var response = await _service.SubmitBatchAsync(request);

        Assert.True(response.IsSuccessful);
        Assert.Empty(response.ItemResponses);
        Assert.Equal(0m, response.TotalAmount);
    }

    [Fact]
    public async Task SubmitBatch_WithSingleItem()
    {
        var request = new AchBatchRequest
        {
            BatchId = 1,
            CompanyId = 1,
            CompanyName = "Test Company",
            CompanyEin = "12-3456789",
            FundingRoutingNumberToken = "ROUTING-123",
            FundingAccountNumberToken = "ACCOUNT-456",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(1),
            SettlementDate = DateTime.UtcNow.Date.AddDays(2),
            SandboxMode = true,
            Items = new List<AchPaymentItem>
            {
                new()
                {
                    EmployeeId = 1,
                    Amount = 5000m,
                    RoutingNumberToken = "ROUTING-001",
                    AccountNumberToken = "ACCOUNT-001",
                    Last4 = "1234",
                    IndividualName = "Solo Employee"
                }
            }
        };

        var response = await _service.SubmitBatchAsync(request);

        Assert.True(response.IsSuccessful);
        Assert.Single(response.ItemResponses);
    }

    [Fact]
    public async Task SubmitBatch_WithLargeAmount()
    {
        var request = CreateBasicBatchRequest();
        request.Items[0].Amount = 999999.99m;

        var response = await _service.SubmitBatchAsync(request);

        Assert.True(response.IsSuccessful);
        Assert.NotEmpty(response.ItemResponses);
    }

    #endregion

    #region Helper Methods

    private AchBatchRequest CreateBasicBatchRequest()
    {
        return new AchBatchRequest
        {
            BatchId = 1,
            CompanyId = 1,
            CompanyName = "Test Company",
            CompanyEin = "12-3456789",
            FundingRoutingNumberToken = "FUNDING-ROUTING-123",
            FundingAccountNumberToken = "FUNDING-ACCOUNT-456",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(1),
            SettlementDate = DateTime.UtcNow.Date.AddDays(2),
            SandboxMode = true,
            Items = new List<AchPaymentItem>
            {
                new()
                {
                    EmployeeId = 1,
                    Amount = 1000m,
                    RoutingNumberToken = "ROUTING-001",
                    AccountNumberToken = "ACCOUNT-001",
                    Last4 = "1234",
                    AccountType = "Checking",
                    IndividualName = "John Doe"
                },
                new()
                {
                    EmployeeId = 2,
                    Amount = 1500m,
                    RoutingNumberToken = "ROUTING-002",
                    AccountNumberToken = "ACCOUNT-002",
                    Last4 = "5678",
                    AccountType = "Savings",
                    IndividualName = "Jane Smith"
                }
            }
        };
    }

    #endregion
}
