namespace BankingApi.Models;

public class RegisterRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Pin { get; set; } = "";
}