using RapidPay.Domain.Models;

namespace RapidPay.Application.Interfaces;

public interface ICardService
{
    Task<Card> CreateCard(string cardNumber, decimal balance);
    Task<bool> ProcessPayment(string cardNumber, decimal amount);
    decimal GetCardBalance(string cardNumber);
    Task<bool> CardExists(string cardNumber);

}