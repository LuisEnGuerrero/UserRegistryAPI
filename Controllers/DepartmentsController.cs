using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System;
using System.Collections.Generic;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        // Configuración de la base de datos inyectada
        private readonly Data.DatabaseConfig _databaseConfig;

        // Constructor que inyecta la configuración de la base de datos
        public DepartmentsController(Data.DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        /// <summary>
        /// Método GET para obtener todos los departamentos.
        /// </summary>
        /// <returns>Una lista de departamentos o un error si ocurre un problema.</returns>
        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            try
            {
                // Crear una conexión a la base de datos utilizando la configuración inyectada
                using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
                {
                    var query = "SELECT * FROM departamento";  // Consulta SQL para obtener los departamentos
                    var departments = dbConnection.Query(query); // Ejecutar la consulta

                    // Validar si se encontraron departamentos
                    if (departments == null || !departments.AsList().Any())
                    {
                        return NotFound("No se encontraron departamentos.");
                    }

                    // Devolver los departamentos encontrados
                    return Ok(departments);
                }
            }
            catch (Exception)
            {
                // Manejo genérico de excepciones: loguear el error y devolver un mensaje genérico al cliente
                // Aquí también podríamos registrar el error en un sistema de logging como Serilog o NLog
                return StatusCode(500, "Ocurrió un error al obtener los departamentos. Por favor, inténtelo de nuevo más tarde.");
            }
        }
    }
}
