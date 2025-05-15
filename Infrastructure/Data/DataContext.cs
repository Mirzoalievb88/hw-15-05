using Npgsql;

namespace Infrastructure.Data;

public class DataContext
{
    private const string connectionString = "Server=localhost;Database=hw-14-05;User Id=postgres;Password=12345;";

    public Task<NpgsqlConnection> GetConnectionAsync()
    {
        return Task.FromResult(new NpgsqlConnection(connectionString));
    }
}