using Microsoft.AspNetCore.Mvc;
using UserRegistryAPI.Data;
using System.Data;
using System;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        // Dependencia inyectada de configuración de la base de datos
        private readonly DatabaseConfig _databaseConfig;

        // Constructor del controlador que inyecta la configuración de la base de datos
        public DatabaseTestController(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        /// <summary>
        /// Método GET para probar la conexión a la base de datos.
        /// </summary>
        /// <returns>Mensaje de éxito si la conexión es exitosa o un mensaje de error en caso de fallo.</returns>
        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            // Crear una conexión con la base de datos
            using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
            {
                try
                {
                    // Intentar abrir la conexión a la base de datos
                    dbConnection.Open();
                    return Ok("Conexión exitosa a la base de datos PostgreSQL.");
                }
                catch (Exception)
                {
                    // Manejar la excepción y devolver un mensaje genérico al cliente
                    // En un entorno real, es mejor no exponer detalles del error.
                    return StatusCode(500, "Error al conectar con la base de datos. Por favor, inténtelo de nuevo más tarde.");
                }
            }
        }
    }
}
