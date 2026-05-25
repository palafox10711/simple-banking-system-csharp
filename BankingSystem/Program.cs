namespace BankingSystem;

class Program
{
    static void Main(string[] args)
    {
        var db = new Database();
        using var connection = db.GetConnection();
        connection.Open();
        Console.WriteLine("Connected to SQL Server successfully!");
    }
}