using SQLite;

namespace BankingApp.Models;

[Table("CustomerAddress")]
public class CustomerAddress
{
    [PrimaryKey, AutoIncrement]
    public int CustomerAddressId { get; set; }

    [MaxLength(100)]
    public string? StreetName { get; set; }

    [MaxLength(10)]
    public string? StreetNumber { get; set; }

    [MaxLength(50)]
    public string? Suburb { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    [MaxLength(10)]
    public string? PostalCode { get; set; }

    [MaxLength(50)]
    public string? Province { get; set; }
}
