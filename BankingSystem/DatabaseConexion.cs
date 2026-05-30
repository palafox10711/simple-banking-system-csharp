namespace BankingSystem;

using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

class Database
{
    private readonly string _connectionString;
    

    public Database()
    {
        string server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost,1433";
        string database = Environment.GetEnvironmentVariable("DB_NAME") ?? "Banking";
        string user = Environment.GetEnvironmentVariable("DB_USER") ??"sa";
        string password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new Exception("DB_PASSWORD not set");
        _connectionString = $"Server={server};Database={database};User id={user};Password={password};TrustServerCertificate=True;";
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}