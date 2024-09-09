using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System;
using System.Collections.Generic; // Para manejar listas de países

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        // Dependencia inyectada de configuración de la base de datos
        private readonly Data.DatabaseConfig _databaseConfig;

        // Constructor del controlador que inyecta la configuración de la base de datos
        public CountriesController(Data.DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        /// <summary>
        /// Método GET para obtener todos los países de la base de datos.
        /// </summary>
        /// <returns>Devuelve la lista de países o un error si ocurre un problema en la consulta.</returns>
        [HttpGet]
        public IActionResult GetAllCountries()
        {
            try
            {
                // Crear una conexión con la base de datos utilizando Dapper
                using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
                {
                    var query = "SELECT * FROM pais";  // Consulta para obtener todos los países

                    // Ejecutar la consulta y obtener los resultados
                    var countries = dbConnection.Query(query);

                    // Verificar si se encontraron países
                    if (countries == null || !countries.AsList().Any())
                    {
                        return NotFound("No se encontraron países.");
                    }

                    // Si se encontraron países, devolverlos
                    return Ok(countries);
                }
            }
            catch (Exception)
            {
                // Manejo genérico de excepciones. Se loguea el error y se devuelve un mensaje genérico al cliente.
                // Aquí también podríamos loguear el error con un sistema de logging (e.g., Serilog, NLog)
                return StatusCode(500, "Ocurrió un error interno. Por favor, inténtalo de nuevo más tarde.");
            }
        }
    }
}
