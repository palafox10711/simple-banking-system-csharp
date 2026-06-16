using Microsoft.AspNetCore.Mvc;
using BankingApi.Models;
using BankingSystem;
using System.Text;                         
using System.Security.Claims;               
using System.IdentityModel.Tokens.Jwt;      
using Microsoft.IdentityModel.Tokens;

namespace BankingApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AccountRepository _repository = new AccountRepository(new Database());

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username is requiered");

        if (request.Password.Length < 8)
            return BadRequest("password must be at least 8 characters");

        if ( request.Pin.Length != 4 || !request.Pin.All(char.IsDigit))
            return BadRequest("Pin must be exactly 4 digits");

        if ( _repository.ExistsUsername(request.Username))
            return Conflict("username is already taken");

        string card = LuhnGenerator.GenerateCardNumber();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        Account account = new Account(0, request.Pin, card, 0, request.Username, passwordHash);
        _repository.Save(account);

        return StatusCode(201, new { username = request.Username, cardNumber = card });
    }

    [HttpPost("login")]
    
    public IActionResult Login([FromBody] LoginRequest request)
    {

        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username is required");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("password is required");

        Account? account = _repository.Login(request.Username, request.Password);
        
        if (account == null)
            return Unauthorized("Invalid username or password");
        
        return Ok(new { token = GenerateToken(account) });
    }

    private string GenerateToken(Account account)
    {
        string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", account.Id.ToString()),
            new Claim("username", account.Username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    


}