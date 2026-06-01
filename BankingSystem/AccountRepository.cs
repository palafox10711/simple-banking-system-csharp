
namespace BankingSystem;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
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
    public Account? FindByCardNumber(string cardnumber)
    {
        using var connection = _db.GetConnection();
        connection.Open();
        using var command = new SqlCommand("SELECT id, pin, card_number, balance FROM card WHERE card_number = @card_number"
        , connection);

        command.Parameters.Add("@card_number", SqlDbType.NVarChar, 16).Value = cardnumber;
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Account(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetDecimal(3));
        }
        return null;
    }
}