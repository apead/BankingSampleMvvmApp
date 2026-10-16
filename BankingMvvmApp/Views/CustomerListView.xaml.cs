using BankingApp.ViewModels;

namespace BankingApp.Views;

public partial class CustomerListView : BaseView
{
    public CustomerListView(CustomerListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
