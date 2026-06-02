
namespace BankingSystem;

class Menu
{
    private readonly AccountRepository? _Repository;

    public Menu(AccountRepository repository)
    {
        _Repository = repository;
    }
    
    public void Run()
    {
        bool isRunning = true;
        while (isRunning)
        {
            Console.WriteLine("\n1. Create account");
            Console.WriteLine("2. Log in");
            Console.WriteLine("0. Exit");
            Console.Write("Enter option: ");

            string? option = Console.ReadLine();
            switch (option)
            {
                case "1": CreateAccount();
                    break;
                case "2":
                    Login();
                    break;
                case "0":
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void CreateAccount()
    {
        Console.WriteLine("Enter yor username, password and pin");
        Console.WriteLine("\n username: ");
        string user = Console.ReadLine() ?? "";
        Console.WriteLine("\n password: ");
        string password = Console.ReadLine() ?? "";
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine("\n pin: ");
        string pin = Console.ReadLine() ?? "";
        var card = LuhnGenerator.GenerateCardNumber();
        var Account = new Account(0, pin, card, 0, user, passwordHash);
        _Repository?.Save(Account);

        System.Console.WriteLine($"Welcome {user} your number card is {card}");
        
    }
    private void Login()
    {
        Console.WriteLine("Enter yor username, password");
        Console.WriteLine("\n username: ");
        string user = Console.ReadLine() ?? "";
        Console.WriteLine("\n password: ");
        string password = Console.ReadLine() ?? "";

        var account = _Repository?.Login(user, password);
        if (account != null)
            Console.WriteLine($"Welcome {account.Username}!");
        else 
            Console.WriteLine("Invalid credentials.");
    
}
}
