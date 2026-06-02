using System.Runtime.InteropServices;

namespace BankingSystem;

class Program
{
    static void Main(string[] args)
    {  
        
        var db = new Database();
        var repository = new AccountRepository(db);
        var cuenta = new Menu(repository);
        cuenta.Run();
    }
    
}