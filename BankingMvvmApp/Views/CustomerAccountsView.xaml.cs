using BankingApp.ViewModels;

namespace BankingApp.Views;

public partial class CustomerAccountsView : BaseView
{
    public CustomerAccountsView(CustomerAccountsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
