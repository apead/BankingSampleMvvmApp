using SQLite;

namespace BankingApp.Models;

[Table("BankAccountType")]
public class BankAccountType
{
    [PrimaryKey, AutoIncrement]
    public int BankAccountTypeId { get; set; }

    [NotNull, MaxLength(50)]
    public string Description { get; set; } = string.Empty;
}
