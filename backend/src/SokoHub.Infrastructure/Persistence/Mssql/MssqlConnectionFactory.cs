using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SokoHub.Infrastructure.Persistence.Mssql;

public class MssqlConnectionFactory
{
    private readonly string _connectionString;

    public MssqlConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("MSSQL connection string 'DefaultConnection' is missing.");
    }

    public async Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
