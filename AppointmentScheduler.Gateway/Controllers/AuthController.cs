using AppointmentScheduler.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentScheduler.Gateway.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest request)
    {
        if (request.Username != "admin" || request.Password != "password")
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var key = _configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT key missing.");
        var issuer = _configuration["JwtSettings:Issuer"] ?? "AppointmentScheduler";
        var audience = _configuration["JwtSettings:Audience"] ?? "AppointmentClients";
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(ClaimTypes.Name, request.Username),
            new Claim("role", "admin")
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Ok(new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token)));
    }
}
