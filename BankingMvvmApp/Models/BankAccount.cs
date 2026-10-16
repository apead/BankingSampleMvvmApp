using SQLite;

namespace BankingApp.Models;

[Table("BankAccount")]
public class BankAccount
{
    [PrimaryKey, AutoIncrement]
    public int BankAccountId { get; set; }

    [NotNull, Unique, MaxLength(20)]
    public string AccountNumber { get; set; } = string.Empty;

    [NotNull]
    public decimal Balance { get; set; }

    // FK -> BankAccountType.BankAccountTypeId
    [NotNull, Indexed]
    public int BankAccountTypeId { get; set; }

    [Ignore]
    public string AccountTypeDescription { get; set; } = string.Empty;

    // FK -> Customer.CustomerId
    [NotNull, Indexed]
    public int CustomerId { get; set; }
}
