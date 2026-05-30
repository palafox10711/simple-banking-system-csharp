namespace BankingSystem;
// id, card_number, pin, balance
class Account(int id, string pin, string cardNumber, decimal balance)
{
    public int Id {get;} = id; 
    public string Pin {get; private set;} = pin;
    public string CardNumber {get;} = cardNumber;
    public decimal Balance {get; private set;} = balance;



}