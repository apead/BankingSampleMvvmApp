using SQLite;

namespace BankingApp.Models;

[Table("Transaction")]
public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int TransactionId { get; set; }

    // Stored as text - open the connection with storeDateTimeAsTicks: false.
    [NotNull]
    public DateTime TransactionDate { get; set; }

    [MaxLength(100)]
    public string? Reference { get; set; }

    // Signed: positive = deposit, negative = withdrawal.
    [NotNull]
    public decimal TransactionAmount { get; set; }

    // FK -> TransactionType.TransactionTypeId
    [NotNull, Indexed]
    public int TransactionTypeId { get; set; }

    [Ignore]
    public string TransactionTypeDescription { get; set; } = string.Empty;

    // FK -> BankAccount.BankAccountId
    [NotNull, Indexed]
    public int BankAccountId { get; set; }
}
