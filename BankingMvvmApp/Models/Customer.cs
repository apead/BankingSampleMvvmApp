using SQLite;

namespace BankingApp.Models;

[Table("Customer")]
public class Customer
{
    [PrimaryKey, AutoIncrement]
    public int CustomerId { get; set; }

    [NotNull, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [NotNull, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(13)]
    public string? IdNumber { get; set; }

    // FK -> Gender.GenderId
    [NotNull, Indexed]
    public int GenderId { get; set; }

    [MaxLength(254)]
    public string? EmailAddress { get; set; }

    // FK -> CustomerAddress.CustomerAddressId (unique: one address per customer)
    [Unique]
    public int? CustomerAddressId { get; set; }
}
