using BankingApp.ViewModels;

namespace BankingApp.Views;

public partial class BankAccountTransactionView : BaseView
{
    public BankAccountTransactionView(BankAccountTransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
