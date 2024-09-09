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

            // Cargar los procedimientos almacenados
            CargarProcedimientos(connection);
        }

    }

    // Método para cargar múltiples procedimientos almacenados
    private void CargarProcedimientos(NpgsqlConnection connection)
    {
        CargarProcedimientoSiNoExiste(connection, "sp_create_user", "StoredProcedures/sp_create_user.sql");
        CargarProcedimientoSiNoExiste(connection, "sp_get_all_users", "StoredProcedures/sp_get_all_users.sql");
        CargarProcedimientoSiNoExiste(connection, "sp_get_user_by_id", "StoredProcedures/sp_get_user_by_id.sql");
        CargarProcedimientoSiNoExiste(connection, "sp_delete_user", "StoredProcedures/sp_delete_user.sql");
        CargarProcedimientoSiNoExiste(connection, "sp_update_user", "StoredProcedures/sp_update_user.sql");
    }


        // Verificar si los procedimientos almacenados ya existen y sino cargarlos
        private void CargarProcedimientoSiNoExiste(NpgsqlConnection connection, string procedureName, string scriptPath)
    {
        var checkProcedureCommand = new NpgsqlCommand(
            $"SELECT EXISTS (SELECT 1 FROM pg_proc WHERE proname = '{procedureName}');",
            connection);
        var procedureExists = checkProcedureCommand.ExecuteScalar() as bool?;

        if (!procedureExists.HasValue || !procedureExists.Value)
        {
            var createProcedureScript = File.ReadAllText(scriptPath);
            var createProcedureCommand = new NpgsqlCommand(createProcedureScript, connection);
            createProcedureCommand.ExecuteNonQuery();
            Console.WriteLine($"Procedimiento almacenado '{procedureName}' creado exitosamente.");
        }
        else
        {
            Console.WriteLine($"El procedimiento almacenado '{procedureName}' ya existe.");
        }
    }
}
