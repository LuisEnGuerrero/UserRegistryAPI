using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System;
using System.Collections.Generic; // Para manejar listas de ciudades

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        // Dependencia inyectada de configuración de la base de datos
        private readonly Data.DatabaseConfig _databaseConfig;

        // Constructor del controlador que inyecta la configuración de la base de datos
        public CitiesController(Data.DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        /// <summary>
        /// Método GET para obtener todas las ciudades (municipios) de la base de datos.
        /// </summary>
        /// <returns>Devuelve la lista de ciudades o un error si ocurre un problema en la consulta.</returns>
        [HttpGet]
        public IActionResult GetAllCities()
        {
            try
            {
                // Crear una conexión con la base de datos utilizando Dapper
                using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
                {
                    var query = "SELECT * FROM municipio";  // Consulta para obtener todos los municipios

                    // Ejecutar la consulta y obtener los resultados
                    var cities = dbConnection.Query(query);

                    // Verificar si se encontraron ciudades
                    if (cities == null || !cities.AsList().Any())
                    {
                        return NotFound("No se encontraron ciudades.");
                    }

                    // Si se encontraron ciudades, devolverlas
                    return Ok(cities);
                }
            }
            catch (Exception)
            {
                // Manejo genérico de excepciones. Se loguea el error y se devuelve un mensaje genérico al cliente.
                // Esto asegura que no se expone información sensible en entornos de producción.
                // Aquí también podríamos loguear el error con un sistema de logging (e.g., Serilog, NLog)
                return StatusCode(500, "Ocurrió un error interno. Por favor, inténtalo de nuevo más tarde.");
            }
        }
    }
}
