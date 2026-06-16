
using System.Runtime;

namespace BankingSystem;

class Menu
{
    private readonly AccountRepository? _Repository;
    private Account? _currentAccount;
    public Menu(AccountRepository repository)
    {
        _Repository = repository;
    }
    
    public void Run()
    {
        bool isRunning = true;
        
        while (isRunning)
        {
            if (_currentAccount == null){
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
        
    
        else {

            Console.WriteLine($"\nHello, {_currentAccount.Username}");
            Console.WriteLine("1. Balance");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Transfer");
            Console.WriteLine("5. Close account");
            Console.WriteLine("0. Log out");
            Console.Write("\nEnter option: ");
            string? option = Console.ReadLine();
            switch (option)
                {
                    case "1": ShowBalance(); break;
                    case "2": Deposit(); break;
                    case "3": Withdraw(); break;
                    case "4": Transfer(); break;
                    case "5": CloseAccount(); break;
                    case "0": Logout(); break;
                    default: Console.WriteLine("Invalid option."); break;
                }
        }
      }  
    }

    private void Logout()
    {
        _currentAccount = null;
        Console.WriteLine("Goodbye!");
    }

    private void ShowBalance()
    {

       var balance = _currentAccount!.Balance;
       Console.WriteLine(balance);
    }
    private void Deposit()
    {
        Console.Write("Amount to deposit: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        { Console.WriteLine("Invalid amount."); return; }

        _currentAccount!.Balance += amount;  
        _Repository!.UpdateBalance(_currentAccount.Id, _currentAccount.Balance);
        Console.WriteLine($"New balance: {_currentAccount.Balance}");
    }
    
    private void Withdraw()
    {
       Console.WriteLine("Amount to Withdraw: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount."); return;
        }

        if (_currentAccount!.Balance < amount)
        {
            Console.WriteLine("Insufficient funds"); return;
        }
        _currentAccount.Balance -= amount;
        _Repository!.UpdateBalance(_currentAccount.Id, _currentAccount.Balance);
        Console.WriteLine($"New balance: {_currentAccount.Balance}");


    }
    private void Transfer()
    {
        Console.WriteLine("enter card number to transfer");
        string cardNumber = Console.ReadLine() ?? "0";
        if (!_Repository!.Exists(cardNumber) || _currentAccount!.CardNumber == cardNumber)
            {Console.WriteLine("invalid card number"); return;}

            Console.WriteLine("enter amont: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amont) || amont <= 0)
        {
                Console.WriteLine("amount invalid");return;
            }
            if (_currentAccount!.Balance >= amont){
            if ( _Repository!.Transfer(_currentAccount!.Id, cardNumber, amont))
            { _currentAccount.Balance -=  amont;
                Console.WriteLine("Transfer completed");}
            else {Console.WriteLine("oops!, something went wrong, please try again");}}
            else {Console.WriteLine("Insufficient funds");}   
    }
    private void CloseAccount()
    {
        if (_currentAccount!.Balance > 0)
        {
            Console.WriteLine("You must withdraw or transfer your balance before closing.");
            return;
        }
        Console.WriteLine("are you sure you want to delete the account? s/n");
        string? input = Console.ReadLine();
        switch (input)
        {
            case "s": Console.WriteLine("enter you password"); 
            string? password = Console.ReadLine();
            if (_Repository!.ValidPassword(_currentAccount!.Username, password!))
                {
                    _Repository.DeleteAccount(_currentAccount.Id);
                    _currentAccount = null;
                    Console.WriteLine("Account deleted successfully. ");
                }
            else {Console.WriteLine("password incorrect");}
            break;
            case "n": return;
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
        if (account != null){
            Console.WriteLine($"Welcome {account.Username}!");
            _currentAccount = account;
        }
        else 
            Console.WriteLine("Invalid credentials.");
    
}
}
