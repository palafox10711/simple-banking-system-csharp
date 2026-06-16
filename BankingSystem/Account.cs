

namespace BankingSystem;
// id, card_number, pin, balance
public class Account(int id, string pin, string cardNumber, decimal balance, string username, string passwordHash)
{
    public int Id {get;} = id; 
    public string Pin {get; private set;} = pin;
    public string CardNumber {get;} = cardNumber;
    public decimal Balance {get; internal set;} = balance;
    public string Username { get; private set; } = username;
    public string PasswordHash {get; private set;} = passwordHash;
}