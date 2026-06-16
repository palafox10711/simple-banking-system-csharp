
namespace BankingSystem;
using Microsoft.Data.SqlClient;
using System.Data;


public class AccountRepository(Database db)
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
        using var command = new SqlCommand("SELECT id, pin, card_number, balance, username FROM card WHERE card_number = @card_number AND is_active = 1"
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
            "SELECT COUNT(*) FROM card WHERE card_number = @card AND is_active = 1", connection
        );
        command.Parameters.Add("@card", SqlDbType.NVarChar,16).Value = card;

        var count = (int)command.ExecuteScalar();
        return count > 0;

        
    }

    public bool ExistsUsername(string username)
    {
        using var connection = _db.GetConnection();
        connection.Open();

        using var command = new SqlCommand (
            "SELECT COUNT(*) FROM card WHERE username = @username", connection
        );
        command.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
        
        var count = (int)command.ExecuteScalar();
        return count > 0;

        
    }

    public Account? Login(string user, string password)

    {
        using var connection = _db.GetConnection();
        connection.Open();

        using var Command = new SqlCommand(
           "SELECT id, pin, card_number, balance , username, password_hash FROM card WHERE username = @user AND is_active = 1", connection
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

    public void UpdateBalance(int id, decimal newBalance)
    {
        
        using var connection = _db.GetConnection();
        connection.Open();

        using var command = new SqlCommand(
            "UPDATE card SET balance = @balance WHERE id = @id", connection
         );
        command.Parameters.Add("@balance", SqlDbType.Decimal).Value = newBalance;
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        command.ExecuteNonQuery();

    }


    public bool Transfer(int id, string cardNumber, decimal amount)
    {
        
            using SqlConnection connection = _db.GetConnection();
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using SqlCommand sender = new SqlCommand ( 
                "UPDATE card SET balance = balance - @amount WHERE id = @id", connection, transaction
            );
            sender.Parameters.Add("@id", SqlDbType.Int).Value = id;
            sender.Parameters.Add("@amount", SqlDbType.Decimal).Value = amount;
            sender.ExecuteNonQuery();
            using SqlCommand recipient = new SqlCommand(
               "UPDATE card SET balance = balance + @amount WHERE card_number = @cardNumber AND is_active = 1", connection, transaction
           );
            recipient.Parameters.Add("@cardNumber", SqlDbType.NVarChar).Value = cardNumber;
            recipient.Parameters.Add("@amount", SqlDbType.Decimal).Value = amount;
            recipient.ExecuteNonQuery();

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

   public bool ValidPassword(string username, string password)
    {
        using SqlConnection connection = _db.GetConnection();
        connection.Open();

        using SqlCommand command = new SqlCommand(
         "SELECT password_hash FROM card WHERE username = @username", connection
        );
        command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return false;
        string password_hash = reader.GetString(0);
        return BCrypt.Net.BCrypt.Verify(password, password_hash);
        
        

    }
    public void DeleteAccount(int id)
    {
        using SqlConnection connection = _db.GetConnection();
        connection.Open();

        using SqlCommand command = new SqlCommand(
         "UPDATE card SET is_active = 0 WHERE id = @id", connection
        );
        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
        command.ExecuteNonQuery();

    }

    public Account? FindById(int id)
    {
        using var connection = _db.GetConnection();
        connection.Open();
        using var command = new SqlCommand("SELECT id, pin, card_number, balance, username FROM card WHERE id = @id AND is_active = 1"
        , connection);

        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
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

}