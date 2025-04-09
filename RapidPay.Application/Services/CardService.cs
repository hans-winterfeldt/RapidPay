using RapidPay.Application.Interfaces;
using RapidPay.Domain.Models;
using RapidPay.Infrastructure.Persistence;

namespace RapidPay.Application.Services;

public class CardService : ICardService
{
    private readonly RapidPayDbContext _dbContext;
    private readonly IFeeService _feeService;

    public CardService(RapidPayDbContext dbContext, IFeeService feeService)
    {
        _dbContext = dbContext;
        _feeService = feeService;
    }

    public async Task<Card> CreateCard(string cardNumber, decimal balance)
    {
        Card card = new()
        {
            CardNumber = cardNumber,
            Balance = balance
        };

        _dbContext.Cards.Add(card);
        await _dbContext.SaveChangesAsync();
        return card;
    }

    public async Task<bool> ProcessPayment(string cardNumber, decimal amount)
    {
        decimal fee = _feeService.GetCurrentFee();
        Card? card = _dbContext.Cards
            .SingleOrDefault(c => c.CardNumber == cardNumber);

        if (card == null || card.Balance < (amount + fee))
        {
            return false;
        }

        card.Balance -= amount + fee;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public decimal GetCardBalance(string cardNumber)
    {
        var card = _dbContext.Cards
            .SingleOrDefault(c => c.CardNumber == cardNumber);

        return card?.Balance ?? 0;
    }

    // New method to validate if a card already exists
    public async Task<bool> CardExists(string cardNumber)
    {
        return await Task.FromResult(_dbContext.Cards.Any(c => c.CardNumber == cardNumber));
    }
}