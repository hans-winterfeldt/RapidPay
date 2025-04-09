using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RapidPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username is required.");
        }

        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
        string? secretKey = jwtSettings["SecretKey"];
        string? issuer = jwtSettings["Issuer"];
        string? audience = jwtSettings["Audience"];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey!));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User")
        };

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}