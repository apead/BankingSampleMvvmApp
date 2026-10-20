using BankingApp.Enums;
using BankingApp.Models;
using BankingApp.ViewModels;
using BankingMvvmApp.Tests.Fakes;

namespace BankingMvvmApp.Tests;

public class TransactionRecordViewModelTests
{
    private readonly FakeBankingDatabaseService database = new();
    private readonly FakeNavigationService navigation = new();
    private readonly FakeDialogService dialogs = new();

    private TransactionRecordViewModel CreateViewModel(TransactionTypeEnum type = TransactionTypeEnum.Deposit)
    {
        var viewModel = new TransactionRecordViewModel(database, navigation, dialogs);
        viewModel.ApplyQueryAttributes(new Dictionary<string, object>
        {
            ["BankAccount"] = new BankAccount { BankAccountId = 7, AccountNumber = "12345", Balance = 100m },
            ["TransactionType"] = type
        });
        return viewModel;
    }

    [Fact]
    public async Task Submit_WithEmptyFields_ShowsValidationErrorsWithoutSaving()
    {
        var viewModel = CreateViewModel();

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.Equal("Please enter a reference.", viewModel.ReferenceError);
        Assert.Equal("Please enter an amount.", viewModel.AmountError);
        Assert.True(viewModel.HasReferenceError);
        Assert.True(viewModel.HasAmountError);
        Assert.Equal(0, dialogs.ConfirmationCount);
        Assert.Equal(0, database.TransactionCount);
    }

    [Theory]
    [InlineData("abc", "Please enter a valid amount.")]
    [InlineData("0", "Amount must be greater than zero.")]
    [InlineData("-5", "Please enter a valid amount.")]
    public async Task Submit_WithInvalidAmount_DoesNotSave(string amount, string expectedError)
    {
        var viewModel = CreateViewModel();
        viewModel.Reference = "Salary";
        viewModel.Amount = amount;

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.Equal(expectedError, viewModel.AmountError);
        Assert.Equal(0, database.TransactionCount);
        Assert.Equal(0, dialogs.ConfirmationCount);
    }

    [Fact]
    public async Task Submit_WithLongReference_DoesNotSave()
    {
        var viewModel = CreateViewModel();
        viewModel.Reference = new string('x', 51);
        viewModel.Amount = "10";

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.Equal("Reference cannot be longer than 50 characters.", viewModel.ReferenceError);
        Assert.Equal(0, database.TransactionCount);
    }

    [Fact]
    public async Task Withdrawal_AboveBalance_ShowsInsufficientFunds()
    {
        var viewModel = CreateViewModel(TransactionTypeEnum.Withdrawl);
        viewModel.Reference = "Groceries";
        viewModel.Amount = "100.01";

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.Equal("Insufficient funds. Amount exceeds the current balance.", viewModel.AmountError);
        Assert.Equal(0, database.TransactionCount);
    }

    [Theory]
    [InlineData(TransactionTypeEnum.Deposit, "150.50", "150.50")]
    [InlineData(TransactionTypeEnum.Deposit, "12,34", "12.34")]
    [InlineData(TransactionTypeEnum.Withdrawl, "100", "100")]
    public async Task ConfirmedTransaction_SavesAndNavigatesBack(
        TransactionTypeEnum type, string amount, string expectedAmount)
    {
        var viewModel = CreateViewModel(type);
        viewModel.Reference = " Test payment ";
        viewModel.Amount = amount;

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.Equal(1, database.TransactionCount);
        Assert.Equal(7, database.LastAccountId);
        Assert.Equal("Test payment", database.LastReference);
        Assert.Equal(decimal.Parse(expectedAmount, System.Globalization.CultureInfo.InvariantCulture),
            database.LastAmount);
        Assert.Equal(type, database.LastTransactionType);
        Assert.Equal(1, dialogs.ConfirmationCount);
        Assert.Equal(1, navigation.GoBackCount);
    }

    [Fact]
    public async Task CancelledTransaction_DoesNotSaveOrNavigate()
    {
        var viewModel = CreateViewModel();
        viewModel.Reference = "Salary";
        viewModel.Amount = "10";
        dialogs.ConfirmResult = false;

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.Equal(1, dialogs.ConfirmationCount);
        Assert.Equal(0, database.TransactionCount);
        Assert.Equal(0, navigation.GoBackCount);
    }

    [Fact]
    public async Task FailedTransaction_ShowsAlertAndAllowsRetry()
    {
        var viewModel = CreateViewModel();
        viewModel.Reference = "Salary";
        viewModel.Amount = "10";
        database.FailTransaction = true;

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.Equal("Transaction Failed", dialogs.LastAlertTitle);
        Assert.Equal("Test database failure.", dialogs.LastAlertMessage);
        Assert.Equal(0, navigation.GoBackCount);
        Assert.True(viewModel.SubmitCommand.CanExecute(null));
    }

    [Fact]
    public void ChangingTransactionType_NotifiesDependentProperties()
    {
        var viewModel = CreateViewModel();
        var propertyNames = new List<string?>();
        viewModel.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

        viewModel.SelectedTransactionType = TransactionTypeEnum.Withdrawl;

        Assert.Contains(nameof(viewModel.SelectedTransactionType), propertyNames);
        Assert.Contains(nameof(viewModel.IsDeposit), propertyNames);
        Assert.Contains(nameof(viewModel.SubmitButtonText), propertyNames);
        Assert.False(viewModel.IsDeposit);
        Assert.Equal("Submit Withdrawal", viewModel.SubmitButtonText);
    }
}
