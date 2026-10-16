using BankingApp.Enums;
using BankingApp.Models;
using BankingApp.Services;
using BankingApp.Commands;
using System.Collections.ObjectModel;

namespace BankingApp.ViewModels;

public class BankAccountTransactionViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IBankingDatabaseService bankingDatabaseService;
    private readonly INavigationService navigationService;
    private readonly IDialogService dialogService;

    public AsyncCommand LoadCommand { get; }
    public AsyncCommand DepositCommand { get; }
    public AsyncCommand WithdrawlCommand { get; }

    private BankAccount? bankAccount;

    public BankAccount? BankAccount
    {
        get { return bankAccount; }
        set
        {
            if (bankAccount == value) return;
            bankAccount = value;
            OnPropertyChanged();
            DepositCommand.NotifyCanExecuteChanged();
            WithdrawlCommand.NotifyCanExecuteChanged();
        }
    }

    private Customer? customer;

    public Customer? Customer
    {
        get { return customer; }
        set
        {
            if (customer == value) return;
            customer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(AccountHolderName));
        }
    }

    private ObservableCollection<Transaction> transactions = new();

    public ObservableCollection<Transaction> Transactions
    {
        get { return transactions; }
        set
        {
            if (transactions == value) return;
            transactions = value;
            OnPropertyChanged();
        }
    }

    public string AccountHolderName => Customer == null
        ? string.Empty
        : $"{Customer.FirstName} {Customer.LastName}".Trim();

    private bool HasBankAccount => BankAccount != null;

    public BankAccountTransactionViewModel(IBankingDatabaseService bankingDatabaseService,
        INavigationService navigationService, IDialogService dialogService)
    {
        this.bankingDatabaseService = bankingDatabaseService;
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        LoadCommand = new AsyncCommand(LoadAsync);
        DepositCommand = new AsyncCommand(DepositAsync, () => HasBankAccount);
        WithdrawlCommand = new AsyncCommand(WithdrawlAsync, () => HasBankAccount);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("BankAccount", out var account))
        {
            BankAccount = account as BankAccount;
        }

        if (query.TryGetValue("Customer", out var holder))
        {
            Customer = holder as Customer;
        }
    }

    public override async Task OnAppearingAsync()
    {
        await LoadCommand.ExecuteAsync(null);
    }

    private async Task LoadAsync()
    {
        if (BankAccount == null)
        {
            return;
        }

        try
        {
            var accountId = BankAccount.BankAccountId;
            var bankingTransactions = await bankingDatabaseService.GetTransactionsByAccountAsync(accountId);
            Transactions = new ObservableCollection<Transaction>(bankingTransactions);
            BankAccount = await bankingDatabaseService.GetBankAccountById(accountId);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            await dialogService.ShowAlertAsync("Unable to load transactions", exception.Message, "OK");
        }
    }

    private Task DepositAsync() => RecordTransactionAsync(TransactionTypeEnum.Deposit);

    private Task WithdrawlAsync() => RecordTransactionAsync(TransactionTypeEnum.Withdrawl);

    private Task RecordTransactionAsync(TransactionTypeEnum transactionType)
    {
        var account = BankAccount ?? throw new InvalidOperationException("No bank account selected.");
        return navigationService.GoToAsync("transactionrecord", new Dictionary<string, object>
        {
            { "BankAccount", account },
            { "TransactionType", transactionType }
        });
    }
}
