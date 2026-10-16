using BankingApp.Enums;
using BankingApp.Models;
using BankingApp.Services;
using BankingApp.Commands;
using System.Globalization;

namespace BankingApp.ViewModels;

public class TransactionRecordViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IBankingDatabaseService bankingDatabaseService;
    private readonly INavigationService navigationService;
    private readonly IDialogService dialogService;

    public AsyncCommand SubmitCommand { get; }

    private BankAccount? bankAccount;

    public BankAccount? BankAccount
    {
        get { return bankAccount; }
        set
        {
            if (bankAccount == value) return;
            bankAccount = value;
            OnPropertyChanged();
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private TransactionTypeEnum selectedTransactionType;

    public TransactionTypeEnum SelectedTransactionType
    {
        get { return selectedTransactionType; }
        set
        {
            if (selectedTransactionType == value) return;
            selectedTransactionType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SubmitButtonText));
            OnPropertyChanged(nameof(IsDeposit));
        }
    }

    private string? reference;

    public string? Reference
    {
        get { return reference; }
        set
        {
            if (reference == value) return;
            reference = value;
            OnPropertyChanged();
        }
    }

    private string? amount;

    public string? Amount
    {
        get { return amount; }
        set
        {
            if (amount == value) return;
            amount = value;
            OnPropertyChanged();
        }
    }

    private string? referenceError;

    public string? ReferenceError
    {
        get { return referenceError; }
        set
        {
            if (referenceError == value) return;
            referenceError = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasReferenceError));
        }
    }

    private string? amountError;

    public string? AmountError
    {
        get { return amountError; }
        set
        {
            if (amountError == value) return;
            amountError = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasAmountError));
        }
    }

    public bool IsDeposit => SelectedTransactionType == TransactionTypeEnum.Deposit;
    public string SubmitButtonText => IsDeposit ? "Submit Deposit" : "Submit Withdrawal";
    public bool HasReferenceError => !string.IsNullOrEmpty(ReferenceError);
    public bool HasAmountError => !string.IsNullOrEmpty(AmountError);
    private bool HasBankAccount => BankAccount != null;

    public TransactionRecordViewModel(IBankingDatabaseService bankingDatabaseService,
        INavigationService navigationService, IDialogService dialogService)
    {
        this.bankingDatabaseService = bankingDatabaseService;
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        SubmitCommand = new AsyncCommand(SubmitAsync, () => HasBankAccount);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        BankAccount = query["BankAccount"] as BankAccount;
        SelectedTransactionType = (TransactionTypeEnum)query["TransactionType"];
    }

    private bool ValidateInput(BankAccount account, out decimal parsedAmount)
    {
        bool isValid = true;
        parsedAmount = 0;
        ReferenceError = null;
        AmountError = null;

        if (string.IsNullOrWhiteSpace(Reference))
        {
            ReferenceError = "Please enter a reference.";
            isValid = false;
        }
        else if (Reference.Trim().Length > 50)
        {
            ReferenceError = "Reference cannot be longer than 50 characters.";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Amount))
        {
            AmountError = "Please enter an amount.";
            isValid = false;
        }
        else
        {
            string normalisedAmount = Amount.Replace(',', '.');
            if (!decimal.TryParse(normalisedAmount, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out parsedAmount))
            {
                AmountError = "Please enter a valid amount.";
                isValid = false;
            }
            else if (parsedAmount <= 0)
            {
                AmountError = "Amount must be greater than zero.";
                isValid = false;
            }
            else if (SelectedTransactionType == TransactionTypeEnum.Withdrawl && parsedAmount > account.Balance)
            {
                AmountError = "Insufficient funds. Amount exceeds the current balance.";
                isValid = false;
            }
        }

        return isValid;
    }

    private async Task SubmitAsync()
    {
        var account = BankAccount ?? throw new InvalidOperationException("No bank account selected.");
        if (!ValidateInput(account, out var parsedAmount))
        {
            return;
        }

        var transactionType = SelectedTransactionType;
        string transactionReference = Reference!.Trim();
        string transactionName = IsDeposit ? "deposit" : "withdrawal";
        string confirmationMessage = string.Format(
            "Are you sure you want to make a {0} of {1:C2} on account {2}?\n\nReference: {3}",
            transactionName, parsedAmount, account.AccountNumber, transactionReference);

        bool confirmed = await dialogService.ConfirmAsync("Confirm Transaction", confirmationMessage, "Yes", "Cancel");
        if (!confirmed)
        {
            return;
        }

        try
        {
            await bankingDatabaseService.MakeTransactionAsync(
                account.BankAccountId, transactionReference, parsedAmount, transactionType);
            await navigationService.GoBackAsync();
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            await dialogService.ShowAlertAsync("Transaction Failed", exception.Message, "OK");
        }
    }
}
