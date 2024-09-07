using Npgsql;
using System;
using System.IO;

public class DatabaseSetupService
{
    private readonly string _connectionString;

    public DatabaseSetupService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InitializeDatabase()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Verifica si la tabla 'pais' ya existe
            var checkTableCommand = new NpgsqlCommand(
                "SELECT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'pais');",
                connection);
            var tableExists = checkTableCommand.ExecuteScalar() as bool?;

            if (!tableExists.HasValue || !tableExists.Value)
            {
                // Si la tabla no existe, ejecuta el script para crear todas las tablas
                var createTablesScript = File.ReadAllText("StoredProcedures/create_tables.sql");
                var createTablesCommand = new NpgsqlCommand(createTablesScript, connection);
                createTablesCommand.ExecuteNonQuery();
                Console.WriteLine("Tablas creadas exitosamente.");
            }
            else
            {
                Console.WriteLine("Las tablas ya existen. No se requiere creación.");
            }
        }
    }
}
