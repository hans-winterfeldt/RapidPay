using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using RapidPay.API.Controllers;
using RapidPay.Application.DTOs;
using RapidPay.Application.Interfaces;
using RapidPay.Domain.Models;
using Shouldly;
using Xunit;

namespace RapidPay.Tests.Controllers;

public class CardsControllerTests
{
    private readonly ICardService _cardService;

    public CardsControllerTests()
    {
        _cardService = A.Fake<ICardService>();
    }

    [Fact]
    public async Task CreateCard_ShouldReturnBadRequest_WhenCardNumberIsInvalid()
    {
        // Arrange
        var controller = GetController();
        var invalidCardDto = new CreateCardDto { CardNumber = "123", Balance = 100 };

        // Act
        var result = await controller.CreateCard(invalidCardDto);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.ShouldBe("Card number must be 15 digits.");
    }

    [Fact]
    public async Task CreateCard_ShouldReturnBadRequest_WhenCardNumberIsNotNumeric()
    {
        // Arrange
        var controller = GetController();
        var invalidCardDto = new CreateCardDto { CardNumber = "12345ABCDE67890", Balance = 100 };

        // Act
        var result = await controller.CreateCard(invalidCardDto);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.ShouldBe("Card number must be numeric.");
    }

    [Fact]
    public async Task CreateCard_ShouldReturnBadRequest_WhenBalanceIsNegative()
    {
        // Arrange
        var controller = GetController();
        var invalidCardDto = new CreateCardDto { CardNumber = "123456789012345", Balance = -100 };

        // Act
        var result = await controller.CreateCard(invalidCardDto);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.ShouldBe("Balance cannot be negative.");
    }

    [Fact]
    public async Task CreateCard_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        var controller = GetController();
        var validCardDto = new CreateCardDto { CardNumber = "123456789012345", Balance = 100 };
        var createdCard = new Card { CardNumber = validCardDto.CardNumber, Balance = validCardDto.Balance };

        A.CallTo(() => _cardService.CreateCard(validCardDto.CardNumber, validCardDto.Balance))
            .Returns(createdCard);

        // Act
        var result = await controller.CreateCard(validCardDto);

        // Assert
        result.ShouldBeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.ShouldBe(createdCard);
    }

    [Fact]
    public async Task CreateCard_ShouldReturnConflict_WhenCardAlreadyExists()
    {
        // Arrange
        var controller = GetController();
        var existingCardDto = new CreateCardDto { CardNumber = "123456789012345", Balance = 100 };

        // Simulate that the card already exists by returning null from the service
        A.CallTo(() => _cardService.CardExists(existingCardDto.CardNumber))
            .Returns(true);

        // Act
        var result = await controller.CreateCard(existingCardDto);

        // Assert
        result.ShouldBeOfType<ConflictObjectResult>();
        var conflictResult = result as ConflictObjectResult;
        conflictResult!.Value.ShouldBe("A card with the given card number already exists.");
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnBadRequest_WhenAmountIsZeroOrNegative()
    {
        // Arrange
        var controller = GetController();
        var cardNumber = "123456789012345";

        // Act
        var result = await controller.ProcessPayment(cardNumber, 0);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.ShouldBe("Payment amount must be greater than zero.");
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnBadRequest_WhenPaymentFails()
    {
        // Arrange
        var controller = GetController();
        var cardNumber = "123456789012345";
        var amount = 100;

        A.CallTo(() => _cardService.ProcessPayment(cardNumber, amount)).Returns(false);

        // Act
        var result = await controller.ProcessPayment(cardNumber, amount);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.ShouldBe("Payment failed. Insufficient balance or card not found.");
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnOk_WhenPaymentSucceeds()
    {
        // Arrange
        var controller = GetController();
        var cardNumber = "123456789012345";
        var amount = 100;

        A.CallTo(() => _cardService.ProcessPayment(cardNumber, amount)).Returns(true);

        // Act
        var result = await controller.ProcessPayment(cardNumber, amount);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBe("Payment processed successfully.");
    }

    private CardsController GetController()
    {
        return new CardsController(_cardService);
    }
}