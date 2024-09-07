using Microsoft.AspNetCore.Mvc;
using UserRegistryAPI.Data;
using System.Data;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public DatabaseTestController(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
            {
                try
                {
                    dbConnection.Open();
                    return Ok("Conexión exitosa a la base de datos PostgreSQL.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al conectar: {ex.Message}");
                }
            }
        }
    }
}
