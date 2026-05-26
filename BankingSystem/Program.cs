using System.Runtime.InteropServices;

namespace BankingSystem;

class Program
{
    static void Main(string[] args)
    {  
        string card = LuhnGenerator.GenerateCardNumber();
        Console.WriteLine(card);
        bool IsValid = LuhnGenerator.IsValid(card);
        Console.WriteLine(IsValid);
        var db = new Database();
        using var connection = db.GetConnection();
        connection.Open();
        Console.WriteLine("Connected to SQL Server successfully!");
    }
}