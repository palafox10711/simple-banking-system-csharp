namespace BankingApi.Models;

public class TransferRequest
{
    public decimal Amount { get; set; }
    public string CardNumber {get; set;} = "";

}