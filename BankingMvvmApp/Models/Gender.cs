using SQLite;

namespace BankingApp.Models;

[Table("Gender")]
public class Gender
{
    [PrimaryKey, AutoIncrement]
    public int GenderId { get; set; }

    [NotNull, MaxLength(50)]
    public string Description { get; set; } = string.Empty;
}
