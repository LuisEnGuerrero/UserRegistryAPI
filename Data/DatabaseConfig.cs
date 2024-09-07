using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace UserRegistryAPI.Data
{
    public class DatabaseConfig
    {
        private readonly string? _connectionString;

        public DatabaseConfig(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
