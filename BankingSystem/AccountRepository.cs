
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
            "INSERT INTO card(card_number, pin, balance, username, password_hash) VALUES (@card_number, @pin, @balance, @username, @password)", connection
        );
        command.Parameters.Add("@card_number", SqlDbType.NVarChar, 16).Value = account.CardNumber;
        command.Parameters.Add("@pin", SqlDbType.NVarChar, 4).Value = account.Pin;
        command.Parameters.Add("@balance", SqlDbType.Decimal).Value = account.Balance;
        command.Parameters.Add("@username", SqlDbType.NVarChar,50).Value = account.Username;
        command.Parameters.Add("@password", SqlDbType.NVarChar,255).Value = account.PasswordHash;
        command.ExecuteNonQuery();
        
    }
    public Account? FindByCardNumber(string cardnumber)
    {
        using var connection = _db.GetConnection();
        connection.Open();
        using var command = new SqlCommand("SELECT id, pin, card_number, balance, username FROM card WHERE card_number = @card_number"
        , connection);

        command.Parameters.Add("@card_number", SqlDbType.NVarChar, 16).Value = cardnumber;
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Account(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetDecimal(3),
            reader.GetString(4),
            "");

        }
        return null;
    }

    public bool Exists(string card)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        using var command = new SqlCommand(
            "SELECT COUNT(*) FROM card WHERE card_number = @card", connection
        );
        command.Parameters.Add("@card", SqlDbType.NVarChar,16).Value = card;

        var count = (int)command.ExecuteScalar();
        return count > 0;

        
    }

    public Account? Login(string user, string password)

    {
        using var connection = _db.GetConnection();
        connection.Open();

        using var Command = new SqlCommand(
           "SELECT id, pin, card_number, balance , username, password_hash FROM card WHERE username = @user", connection
        );
        Command.Parameters.Add("@user", SqlDbType.NVarChar, 50).Value = user;
        using var reader = Command.ExecuteReader();
        if (reader.Read())
        {
            var passwordHash = reader.GetString(5);
            if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
            {
                return new Account(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetDecimal(3),
                    reader.GetString(4),
                    "");
            }
        }
        return null;
    }
    
}