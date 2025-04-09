namespace RapidPay.Application.DTOs;

public class CreateCardDto
{
    public string CardNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
}