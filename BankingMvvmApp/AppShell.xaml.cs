using BankingApp.Views;

namespace BankingApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("customeraccounts", typeof(CustomerAccountsView));
            Routing.RegisterRoute("bankaccounttransactions", typeof(BankAccountTransactionView));
            Routing.RegisterRoute("transactionrecord", typeof(TransactionRecordView));
        }
    }
}
