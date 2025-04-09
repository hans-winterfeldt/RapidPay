using Bogus;
using FakeItEasy;
using RapidPay.Application.Interfaces;
using RapidPay.Application.Services;
using RapidPay.Infrastructure.Persistence;
using RapidPay.Tests.Factories;
using Shouldly;

namespace RapidPay.Tests.Services;

public class CardServiceTests
{
    private readonly IFeeService _feeService;
    private readonly RapidPayDbContext _dbContext;
    private readonly Faker _faker = new();

    public CardServiceTests()
    {
        _dbContext = InMemoryDbContextFactory.CreateDbContext();
        _feeService = A.Fake<IFeeService>();
    }

    [Fact]
    public async Task CreateCard_ShouldAddCardToDatabase()
    {
        // Arrange
        string cardNumber = _faker.Random.Replace("###############");
        decimal initialBalance = _faker.Random.Decimal(1, 1000);
        CardService cardService = GetService();

        // Act
        Domain.Models.Card card = await cardService.CreateCard(cardNumber, initialBalance);

        // Assert
        card.ShouldNotBeNull();
        card.CardNumber.ShouldBe(cardNumber);
        card.Balance.ShouldBe(initialBalance);
    }

    [Fact]
    public async Task ProcessPayment_ShouldDeductBalance()
    {
        // Arrange
        string cardNumber = _faker.Random.Replace("###############");
        Domain.Models.Card card = new()
        {
            CardNumber = cardNumber,
            Balance = 1000
        };
        _dbContext.Cards.Add(card);
        await _dbContext.SaveChangesAsync();

        A.CallTo(() => _feeService.GetCurrentFee()).Returns(20); // Mocking fee service to return a fee of 20m

        CardService cardService = GetService();

        // Act
        bool result = await cardService.ProcessPayment(cardNumber, 200);

        // Assert
        result.ShouldBeTrue();
        Domain.Models.Card resultCard = _dbContext.Cards
            .First(c => c.CardNumber == cardNumber);

        card.Balance.ShouldBe(780);
    }

    [Fact]
    public async Task CardExists_ShouldReturnTrue_WhenCardExists()
    {
        // Arrange
        string cardNumber = _faker.Random.Replace("###############");
        Domain.Models.Card existingCard = new()
        {
            CardNumber = cardNumber,
            Balance = _faker.Random.Decimal(1, 1000)
        };
        _dbContext.Cards.Add(existingCard);
        await _dbContext.SaveChangesAsync();

        CardService cardService = GetService();

        // Act
        bool exists = await cardService.CardExists(cardNumber);

        // Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task CardExists_ShouldReturnFalse_WhenCardDoesNotExist()
    {
        // Arrange
        string cardNumber = _faker.Random.Replace("###############");
        CardService cardService = GetService();

        // Act
        bool exists = await cardService.CardExists(cardNumber);

        // Assert
        exists.ShouldBeFalse();
    }

    private CardService GetService()
    {
        return new CardService(_dbContext, _feeService);
    }
}