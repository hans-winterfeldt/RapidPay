using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RapidPay.API.Controllers;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RapidPay.Tests.Controllers;

public class AuthControllerTests
{
    private readonly IConfiguration _configuration;

    public AuthControllerTests()
    {
        // Mock IConfiguration with test JWT settings
        Dictionary<string, string?> inMemorySettings = new()
        {
            { "JwtSettings:SecretKey", "TestSecretKey12345678901234567890123456789012" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        string username = "TestUser";
        var authController = GetController();

        // Act
        IActionResult result = authController.GenerateToken(username);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<OkObjectResult>();

        OkObjectResult? okResult = result as OkObjectResult;
        okResult!.Value.ShouldNotBeNull();
        okResult.Value.ShouldBeOfType<string>();

        string? token = okResult.Value as string;
        ValidateToken(token, username).ShouldBeTrue();
    }

    [Fact]
    public void GenerateToken_ShouldReturnBadRequest_WhenUsernameIsEmpty()
    {
        // Arrange
        var authController = GetController();

        // Act
        IActionResult result = authController.GenerateToken("");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BadRequestObjectResult>();

        BadRequestObjectResult? badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.ShouldBe("Username is required.");
    }

    private AuthController GetController()
    {
        return new AuthController(_configuration);
    }

    private bool ValidateToken(string? token, string expectedUsername)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
        string? secretKey = jwtSettings["SecretKey"];
        string? issuer = jwtSettings["Issuer"];
        string? audience = jwtSettings["Audience"];

        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.UTF8.GetBytes(secretKey!);

        try
        {
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken? validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            Claim? usernameClaim = principal.FindFirst(ClaimTypes.Name);
            return usernameClaim != null && usernameClaim.Value == expectedUsername;
        }
        catch
        {
            return false;
        }
    }
}