using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidPay.Application.DTOs;
using RapidPay.Application.Interfaces;
using RapidPay.Domain.Models;

namespace RapidPay.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] CreateCardDto createCardDto)
    {
        if (string.IsNullOrWhiteSpace(createCardDto.CardNumber) || createCardDto.CardNumber.Length != 15)
        {
            return BadRequest("Card number must be 15 digits.");
        }

        if (!long.TryParse(createCardDto.CardNumber, out _))
        {
            return BadRequest("Card number must be numeric.");
        }

        if (createCardDto.Balance < 0)
        {
            return BadRequest("Balance cannot be negative.");
        }

        if (await _cardService.CardExists(createCardDto.CardNumber))
        {
            return Conflict("A card with the given card number already exists.");
        }

        Card card = await _cardService.CreateCard(createCardDto.CardNumber, createCardDto.Balance);

        return CreatedAtAction(nameof(GetCardBalance), new { cardNumber = card.CardNumber }, card);
    }

    [HttpGet("{cardNumber}/balance")]
    public IActionResult GetCardBalance(string cardNumber)
    {
        decimal balance = _cardService.GetCardBalance(cardNumber);
        return balance == 0 ? NotFound("Card not found or balance is zero.") : Ok(balance);
    }

    [HttpPost("{cardNumber}/pay")]
    public async Task<IActionResult> ProcessPayment(string cardNumber, [FromBody] decimal amount)
    {
        if (amount <= 0)
        {
            return BadRequest("Payment amount must be greater than zero.");
        }

        bool success = await _cardService.ProcessPayment(cardNumber, amount);

        return !success ? BadRequest("Payment failed. Insufficient balance or card not found.") : Ok("Payment processed successfully.");
    }
}