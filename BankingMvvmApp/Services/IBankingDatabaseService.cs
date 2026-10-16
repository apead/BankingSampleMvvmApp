using BankingApp.Enums;
using BankingApp.Models;

namespace BankingApp.Services;

public interface IBankingDatabaseService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<List<Customer>> SearchCustomersByLastNameAsync(string? searchText);
    Task<List<BankAccount>> GetAccountsByCustomerAsync(int customerId);
    Task<BankAccount> GetBankAccountById(int bankAccountId);
    Task<List<Transaction>> GetTransactionsByAccountAsync(int bankAccountId);
    Task MakeTransactionAsync(int bankAccountId, string reference, decimal amount, TransactionTypeEnum transactionType);
}
