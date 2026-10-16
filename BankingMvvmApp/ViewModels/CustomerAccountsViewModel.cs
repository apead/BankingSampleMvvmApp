using BankingApp.Models;
using BankingApp.Services;
using BankingApp.Commands;
using System.Collections.ObjectModel;

namespace BankingApp.ViewModels;

public class CustomerAccountsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IBankingDatabaseService bankingDatabaseService;
    private readonly INavigationService navigationService;
    private readonly IDialogService dialogService;

    public AsyncCommand LoadAccountsCommand { get; }
    public AsyncCommand SelectAccountCommand { get; }

    private Customer? customer;

    public Customer? Customer
    {
        get { return customer; }
        set
        {
            if (customer == value) return;
            customer = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustomerFullName));
        }
    }

    private ObservableCollection<BankAccount> customerAccounts = new();

    public ObservableCollection<BankAccount> CustomerAccounts
    {
        get { return customerAccounts; }
        set
        {
            if (customerAccounts == value) return;
            customerAccounts = value;
            OnPropertyChanged();
        }
    }

    private BankAccount? selectedAccount;

    public BankAccount? SelectedAccount
    {
        get { return selectedAccount; }
        set
        {
            if (selectedAccount == value) return;
            selectedAccount = value;
            OnPropertyChanged();
        }
    }

    public string CustomerFullName => Customer == null
        ? string.Empty
        : $"{Customer.FirstName} {Customer.LastName}".Trim();

    public CustomerAccountsViewModel(IBankingDatabaseService bankingDatabaseService,
        INavigationService navigationService, IDialogService dialogService)
    {
        this.bankingDatabaseService = bankingDatabaseService;
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        LoadAccountsCommand = new AsyncCommand(LoadAccountsAsync);
        SelectAccountCommand = new AsyncCommand(SelectAccountAsync);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Customer", out var value))
        {
            Customer = value as Customer;
        }
    }

    public override async Task OnAppearingAsync()
    {
        await LoadAccountsCommand.ExecuteAsync(null);
    }

    private async Task LoadAccountsAsync()
    {
        if (Customer == null)
        {
            return;
        }

        try
        {
            var accounts = await bankingDatabaseService.GetAccountsByCustomerAsync(Customer.CustomerId);
            CustomerAccounts = new ObservableCollection<BankAccount>(accounts);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            await dialogService.ShowAlertAsync("Unable to load accounts", exception.Message, "OK");
        }
    }

    private async Task SelectAccountAsync()
    {
        var account = SelectedAccount;
        SelectedAccount = null;
        if (account == null)
        {
            return;
        }

        var parameters = new Dictionary<string, object> { { "BankAccount", account } };
        if (Customer != null)
        {
            parameters.Add("Customer", Customer);
        }

        await navigationService.GoToAsync("bankaccounttransactions", parameters);
    }
}
