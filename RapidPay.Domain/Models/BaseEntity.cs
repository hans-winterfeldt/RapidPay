using System.ComponentModel.DataAnnotations.Schema;

namespace RapidPay.Domain.Models;

public partial class BaseEntity
{
    [Column(Order = 0)]
    public int Id { get; set; }

    [Column(Order = 1)]
    public DateTime CreatedOnUtc { get; set; }

    [Column(Order = 2)]
    public string CreatedBy { get; set; } = "system";

    [Column(Order = 3)]
    public DateTime ModifiedOnUtc { get; set; }

    [Column(Order = 4)]
    public string ModifiedBy { get; set; } = "system";

    [Column(Order = 5)]
    public ulong RowVersion { get; set; }

    public BaseEntity()
    {
        CreatedOnUtc = DateTime.UtcNow;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}