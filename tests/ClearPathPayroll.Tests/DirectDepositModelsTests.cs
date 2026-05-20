using ClearPathPayroll.Domain;
using Xunit;

namespace ClearPathPayroll.Tests;

public class DirectDepositModelsTests
{
    [Fact]
    public void EmployeeBankAccount_CanInitializeWithTokensAndLast4()
    {
        var account = new EmployeeBankAccount
        {
            EmployeeId = 1,
            BankName = "Test Bank",
            AccountType = BankAccountType.Checking,
            RoutingNumberToken = "ROUTING-TOKEN-123",
            AccountNumberToken = "ACCOUNT-TOKEN-456",
            Last4 = "1234",
            VerificationStatus = VerificationStatus.Verified,
            IsActive = true,
            CreatedByUserId = "tester"
        };

        Assert.Equal(1, account.EmployeeId);
        Assert.Equal("Test Bank", account.BankName);
        Assert.Equal(BankAccountType.Checking, account.AccountType);
        Assert.Equal("ROUTING-TOKEN-123", account.RoutingNumberToken);
        Assert.Equal("ACCOUNT-TOKEN-456", account.AccountNumberToken);
        Assert.Equal("1234", account.Last4);
        Assert.True(account.IsActive);
    }

    [Fact]
    public void CompanyFundingAccount_CanInitializeWithTokenizedAccount()
    {
        var account = new CompanyFundingAccount
        {
            CompanyId = 1,
            BankName = "Funding Bank",
            AccountType = BankAccountType.Checking,
            RoutingNumberToken = "FUND-ROUTING-TOKEN",
            AccountNumberToken = "FUND-ACCOUNT-TOKEN",
            Last4 = "5678",
            Description = "Primary funding account",
            IsPrimary = true,
            IsActive = true,
            CreatedByUserId = "tester"
        };

        Assert.Equal(1, account.CompanyId);
        Assert.True(account.IsPrimary);
        Assert.Equal("5678", account.Last4);
    }

    [Fact]
    public void DirectDepositBatch_TotalAmountCalculationIsCorrect()
    {
        var batch = new DirectDepositBatch
        {
            BatchId = 1,
            CompanyId = 1,
            PayrollRunId = 1,
            CompanyFundingAccountId = 1,
            PayDate = DateTime.UtcNow.Date.AddDays(2),
            Status = DirectDepositBatchStatus.Draft,
            ExternalBatchReference = "BATCH-123",
            CreatedByUserId = "tester"
        };

        batch.Items.Add(new DirectDepositItem
        {
            EmployeeId = 1,
            EmployeeBankAccountId = 1,
            Amount = 1000m,
            Status = DirectDepositItemStatus.Pending,
            ExternalItemReference = "ITEM-1"
        });

        batch.Items.Add(new DirectDepositItem
        {
            EmployeeId = 2,
            EmployeeBankAccountId = 2,
            Amount = 1500m,
            Status = DirectDepositItemStatus.Pending,
            ExternalItemReference = "ITEM-2"
        });

        batch.TotalAmount = batch.Items.Sum(i => i.Amount);

        Assert.Equal(2500m, batch.TotalAmount);
        Assert.Equal(2, batch.ItemCount);
    }

    [Fact]
    public void DirectDepositItem_StatusAndReferencesAreStored()
    {
        var item = new DirectDepositItem
        {
            BatchId = 1,
            EmployeeId = 1,
            EmployeeBankAccountId = 1,
            Amount = 1200.50m,
            Status = DirectDepositItemStatus.Transmitted,
            ExternalItemReference = "REF-100",
            ReturnCode = "R01",
            ReturnDescription = "Insufficient funds"
        };

        Assert.Equal(1200.50m, item.Amount);
        Assert.Equal(DirectDepositItemStatus.Transmitted, item.Status);
        Assert.Equal("REF-100", item.ExternalItemReference);
        Assert.Equal("R01", item.ReturnCode);
        Assert.Equal("Insufficient funds", item.ReturnDescription);
    }
}
