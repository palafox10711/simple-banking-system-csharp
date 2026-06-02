using System.Runtime.InteropServices;

namespace BankingSystem;

class Program
{
    static void Main(string[] args)
    {  
        var db = new Database();
        var repository = new AccountRepository(db);
        string card = LuhnGenerator.GenerateCardNumber();
        if (LuhnGenerator.IsValid(card))
        {
            var acount = new Account(0, "4532", card, 0);
            repository.Save(acount);
            System.Console.WriteLine(card);
        }
        var found = repository.FindByCardNumber("4427163735981022");
        Console.WriteLine(found?.CardNumber);
        Console.WriteLine(repository.Exists("4427163735981022")); // true
        Console.WriteLine(repository.Exists("0000000000000000")); // false


    }
    
}