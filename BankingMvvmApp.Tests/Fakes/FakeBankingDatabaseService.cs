using BankingApp.Enums;
using BankingApp.Models;
using BankingApp.Services;

namespace BankingMvvmApp.Tests.Fakes;

internal class FakeBankingDatabaseService : IBankingDatabaseService
{
    public List<Customer> Customers { get; set; } = [];
    public List<BankAccount> Accounts { get; set; } = [];
    public List<Transaction> Transactions { get; set; } = [];
    public BankAccount Account { get; set; } = new();
    public string? LastSearchText { get; private set; }
    public int? LastCustomerId { get; private set; }
    public int TransactionCount { get; private set; }
    public int LastAccountId { get; private set; }
    public string? LastReference { get; private set; }
    public decimal LastAmount { get; private set; }
    public TransactionTypeEnum LastTransactionType { get; private set; }
    public bool FailTransaction { get; set; }

    public Task<List<Customer>> GetAllCustomersAsync() => Task.FromResult(Customers);

    public Task<List<Customer>> SearchCustomersByLastNameAsync(string? searchText)
    {
        LastSearchText = searchText;
        return Task.FromResult(Customers);
    }

    public Task<List<BankAccount>> GetAccountsByCustomerAsync(int customerId)
    {
        LastCustomerId = customerId;
        return Task.FromResult(Accounts);
    }

    public Task<BankAccount> GetBankAccountById(int bankAccountId) => Task.FromResult(Account);

    public Task<List<Transaction>> GetTransactionsByAccountAsync(int bankAccountId) =>
        Task.FromResult(Transactions);

    public Task MakeTransactionAsync(int bankAccountId, string reference, decimal amount,
        TransactionTypeEnum transactionType)
    {
        if (FailTransaction)
        {
            throw new InvalidOperationException("Test database failure.");
        }

        TransactionCount++;
        LastAccountId = bankAccountId;
        LastReference = reference;
        LastAmount = amount;
        LastTransactionType = transactionType;
        return Task.CompletedTask;
    }
}
