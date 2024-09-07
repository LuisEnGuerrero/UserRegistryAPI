using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly Data.DatabaseConfig _databaseConfig;

        public CitiesController(Data.DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        [HttpGet]
        public IActionResult GetAllCities()
        {
            using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
            {
                var query = "SELECT * FROM municipio";
                var cities = dbConnection.Query(query);
                return Ok(cities);
            }
        }
    }
}
