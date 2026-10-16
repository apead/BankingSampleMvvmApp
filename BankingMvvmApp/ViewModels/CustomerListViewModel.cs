using BankingApp.Models;
using BankingApp.Services;
using BankingApp.Commands;
using System.Collections.ObjectModel;

namespace BankingApp.ViewModels;

public class CustomerListViewModel : BaseViewModel
{
    private readonly IBankingDatabaseService bankingDatabaseService;
    private readonly INavigationService navigationService;
    private readonly IDialogService dialogService;
    private int customerSearchVersion;

    public AsyncCommand LoadCustomersCommand { get; }
    public AsyncCommand SelectCustomerCommand { get; }

    private ObservableCollection<Customer> customers = new();

    public ObservableCollection<Customer> Customers
    {
        get { return customers; }
        set
        {
            if (customers == value) return;
            customers = value;
            OnPropertyChanged();
        }
    }

    private string? searchText;

    public string? SearchText
    {
        get { return searchText; }
        set
        {
            if (searchText == value) return;
            searchText = value;
            OnPropertyChanged();
            LoadCustomersCommand.Execute(null);
        }
    }

    private Customer? selectedCustomer;

    public Customer? SelectedCustomer
    {
        get { return selectedCustomer; }
        set
        {
            if (selectedCustomer == value) return;
            selectedCustomer = value;
            OnPropertyChanged();
        }
    }

    public CustomerListViewModel(IBankingDatabaseService bankingDatabaseService,
        INavigationService navigationService, IDialogService dialogService)
    {
        this.bankingDatabaseService = bankingDatabaseService;
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        LoadCustomersCommand = new AsyncCommand(LoadCustomersAsync, allowConcurrentExecutions: true);
        SelectCustomerCommand = new AsyncCommand(SelectCustomerAsync);
    }

    public override async Task OnAppearingAsync()
    {
        await LoadCustomersCommand.ExecuteAsync(null);
    }

    private async Task LoadCustomersAsync()
    {
        int searchVersion = ++customerSearchVersion;
        string name = SearchText?.Trim() ?? string.Empty;

        if (name.Length > 0 && name.Length < 3)
        {
            return;
        }

        try
        {
            var matchingCustomers = await bankingDatabaseService.SearchCustomersByLastNameAsync(name);
            if (searchVersion == customerSearchVersion)
            {
                Customers = new ObservableCollection<Customer>(matchingCustomers);
            }
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            if (searchVersion == customerSearchVersion)
            {
                await dialogService.ShowAlertAsync("Unable to load customers", exception.Message, "OK");
            }
        }
    }

    private async Task SelectCustomerAsync()
    {
        var customer = SelectedCustomer;
        SelectedCustomer = null;
        if (customer != null)
        {
            await navigationService.GoToAsync("customeraccounts", new Dictionary<string, object>
            {
                { "Customer", customer }
            });
        }
    }
}
