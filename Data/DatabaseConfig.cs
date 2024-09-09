using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace UserRegistryAPI.Data
{
    public class DatabaseConfig
    {
        // Cadena de conexión de la base de datos
        private readonly string _connectionString;

        /// <summary>
        /// Constructor que obtiene la cadena de conexión de la configuración.
        /// </summary>
        /// <param name="configuration">Configuración del entorno (appsettings.json, variables de entorno, etc.)</param>
        public DatabaseConfig(IConfiguration configuration)
        {
            // Obtener la cadena de conexión desde la configuración
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("La cadena de conexión 'DefaultConnection' no está configurada.");
        }

        /// <summary>
        /// Crear y devolver una nueva conexión a la base de datos utilizando Npgsql (PostgreSQL).
        /// </summary>
        /// <returns>Una nueva conexión a la base de datos.</returns>
        public IDbConnection CreateConnection()
        {
            // Verificar si la cadena de conexión no es nula o vacía antes de crear la conexión
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no puede estar vacía.");
            }

            // Retornar una nueva instancia de NpgsqlConnection utilizando la cadena de conexión
            return new NpgsqlConnection(_connectionString);
        }
    }
}
