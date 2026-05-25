namespace BankingSystem;

using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

class Database
{
    private readonly string _connectionString;

    public Database()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string baseConnection = config.GetConnectionString("Banking")
            ?? throw new InvalidOperationException("Connection string not found.");

        string password = Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException("DB_PASSWORD not set.");

        _connectionString = baseConnection + $"Password={password};";
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}

    