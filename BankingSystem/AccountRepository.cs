
namespace BankingSystem;
using Microsoft.Data.SqlClient;
using System.Data;
class AccountRepository(Database db)
{
    private readonly Database _db = db;
    public void Save(Account account)
    {
        using var connection = _db.GetConnection();
        connection.Open();
        
        using var command = new SqlCommand(
            "INSERT INTO card(card_number, pin, balance) VALUES (@card_number, @pin, @balance)", connection
        );
        command.Parameters.Add("@card_number", SqlDbType.NVarChar, 16).Value = account.CardNumber;
        command.Parameters.Add("@pin", SqlDbType.NVarChar, 4).Value = account.Pin;
        command.Parameters.Add("@balance", SqlDbType.Decimal).Value = account.Balance;
        command.ExecuteNonQuery();
        
    }
}