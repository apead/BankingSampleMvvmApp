using SQLite;

namespace BankingApp.Models;

[Table("TransactionType")]
public class TransactionType
{
    [PrimaryKey, AutoIncrement]
    public int TransactionTypeId { get; set; }

    [NotNull, MaxLength(50)]
    public string Description { get; set; } = string.Empty;
}
