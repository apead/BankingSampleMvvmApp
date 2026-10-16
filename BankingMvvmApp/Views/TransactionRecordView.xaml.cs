using BankingApp.ViewModels;

namespace BankingApp.Views;

public partial class TransactionRecordView : BaseView
{
    public TransactionRecordView(TransactionRecordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
