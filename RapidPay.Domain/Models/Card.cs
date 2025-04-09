using System.ComponentModel.DataAnnotations;

namespace RapidPay.Domain.Models;
public class Card : BaseEntity
{
    public string CardNumber { get; set; } = string.Empty;

    [ConcurrencyCheck]
    public decimal Balance { get; set; } = 0;
}