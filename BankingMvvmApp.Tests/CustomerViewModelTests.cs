using BankingApp.Models;
using BankingApp.ViewModels;
using BankingMvvmApp.Tests.Fakes;

namespace BankingMvvmApp.Tests;

public class CustomerViewModelTests
{
    [Fact]
    public async Task Appearing_LoadsCustomers()
    {
        var customer = new Customer { CustomerId = 1, FirstName = "Ada", LastName = "Test" };
        var database = new FakeBankingDatabaseService { Customers = [customer] };
        var viewModel = new CustomerListViewModel(database, new FakeNavigationService(), new FakeDialogService());

        await viewModel.OnAppearingAsync();

        Assert.Same(customer, Assert.Single(viewModel.Customers));
        Assert.Equal(string.Empty, database.LastSearchText);
    }

    [Fact]
    public async Task SelectingCustomer_ClearsSelectionAndNavigatesWithCustomer()
    {
        var navigation = new FakeNavigationService();
        var viewModel = new CustomerListViewModel(new FakeBankingDatabaseService(), navigation, new FakeDialogService());
        var customer = new Customer { CustomerId = 1 };
        viewModel.SelectedCustomer = customer;

        await viewModel.SelectCustomerCommand.ExecuteAsync(null);

        Assert.Null(viewModel.SelectedCustomer);
        Assert.Equal("customeraccounts", navigation.LastRoute);
        Assert.Same(customer, navigation.LastParameters["Customer"]);
    }

    [Fact]
    public async Task Appearing_LoadsAccountsForSelectedCustomer()
    {
        var account = new BankAccount { BankAccountId = 7 };
        var database = new FakeBankingDatabaseService { Accounts = [account] };
        var viewModel = new CustomerAccountsViewModel(database, new FakeNavigationService(), new FakeDialogService());
        viewModel.ApplyQueryAttributes(new Dictionary<string, object>
        {
            ["Customer"] = new Customer { CustomerId = 42, FirstName = "Ada", LastName = "Test" }
        });

        await viewModel.OnAppearingAsync();

        Assert.Equal(42, database.LastCustomerId);
        Assert.Same(account, Assert.Single(viewModel.CustomerAccounts));
        Assert.Equal("Ada Test", viewModel.CustomerFullName);
    }
}
