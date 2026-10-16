using BankingApp.Enums;
using BankingApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankingApp.Services
{
    public class BankingDatabaseService : IBankingDatabaseService
    {
        private string dbFileName = "banking.db";
        private string localDbPath = string.Empty;
        private SQLiteAsyncConnection dbConnection;

        public BankingDatabaseService()
        {
            localDbPath = Path.Combine(FileSystem.Current.AppDataDirectory, dbFileName);
        }

        private async Task<SQLiteAsyncConnection> GetDbConnectionAsync()
        {
            if (dbConnection != null)
            {
                return dbConnection;
            }
                 

            if (!File.Exists(localDbPath))
                await ExtractDbfromMauiAssetAsync();

            dbConnection = new SQLiteAsyncConnection(localDbPath, storeDateTimeAsTicks: false);

            return dbConnection;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            SQLiteAsyncConnection connection = await GetDbConnectionAsync();

            return await connection.Table<Customer>().ToListAsync();
        }

        public async Task<BankAccount> GetBankAccountById(int bankAccountId)
        {
            SQLiteAsyncConnection connection = await GetDbConnectionAsync();

            return await connection.Table<BankAccount>().Where(x => x.BankAccountId == bankAccountId).FirstOrDefaultAsync();
        }

        public async Task<List<Customer>> SearchCustomersByLastNameAsync(string? searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return await GetAllCustomersAsync();
            }

            SQLiteAsyncConnection connection = await GetDbConnectionAsync();
            string name = searchText.Trim();
            string pattern = "%" + name + "%";

            return await connection.QueryAsync<Customer>(
                @"SELECT * FROM Customer
                  WHERE LastName LIKE ?",
                pattern);
        }

        public async Task<List<BankAccount>> GetAccountsByCustomerAsync(int customerId)
        {
            SQLiteAsyncConnection connection = await GetDbConnectionAsync();
            
            var accounts = await connection.Table<BankAccount>()
                .Where(bankAccount => bankAccount.CustomerId == customerId)
                .ToListAsync();
            
            var accountTypes = (await connection.Table<BankAccountType>().ToListAsync())
                .ToDictionary(type => type.BankAccountTypeId);

            foreach (var account in accounts)
            {
                if (!accountTypes.TryGetValue(account.BankAccountTypeId, out var accountType))
                {
                    throw new InvalidOperationException(
                        $"Account {account.BankAccountId} references missing account type {account.BankAccountTypeId}.");
                }

                account.AccountTypeDescription = accountType.Description;
            }

            return accounts;
        }

        public async Task<List<Transaction>> GetTransactionsByAccountAsync(int bankAccountId)
        {
            SQLiteAsyncConnection connection = await GetDbConnectionAsync();

            var transactions = await connection.Table<Transaction>()
                .Where(t => t.BankAccountId == bankAccountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
            var transactionTypes = (await connection.Table<TransactionType>().ToListAsync())
                .ToDictionary(type => type.TransactionTypeId);

            foreach (var transaction in transactions)
            {
                if (!transactionTypes.TryGetValue(transaction.TransactionTypeId, out var transactionType))
                {
                    throw new InvalidOperationException(
                        $"Transaction {transaction.TransactionId} references missing transaction type {transaction.TransactionTypeId}.");
                }

                transaction.TransactionTypeDescription = transactionType.Description;
            }

            return transactions;
        }

        public async Task MakeTransactionAsync(int bankAccountId, string reference, decimal amount, TransactionTypeEnum transactionType)
        {
            if(transactionType == TransactionTypeEnum.Withdrawl)
            {
                amount = -amount;
            }

            SQLiteAsyncConnection connection = await GetDbConnectionAsync();


            BankAccount account = await connection.Table<BankAccount>().Where(x => x.BankAccountId == bankAccountId).FirstOrDefaultAsync();

            account.Balance += amount;

            await connection.UpdateAsync(account);

            Transaction transaction = new Transaction();
            transaction.TransactionDate = DateTime.Now;
            transaction.Reference = reference;
            transaction.BankAccountId = bankAccountId;
            transaction.TransactionAmount = amount;
            transaction.TransactionTypeId = (int) transactionType;

            await connection.InsertAsync(transaction);

          
        }


        private async Task ExtractDbfromMauiAssetAsync()
        {
            using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(dbFileName);
            
            // Copy the file to the AppDataDirectory
            using FileStream outputStream = File.Create(localDbPath);
            await inputStream.CopyToAsync(outputStream);

        }
    }
}
