using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly Data.DatabaseConfig _databaseConfig;

        public CountriesController(Data.DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        [HttpGet]
        public IActionResult GetAllCountries()
        {
            using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
            {
                var query = "SELECT * FROM pais";
                var countries = dbConnection.Query(query);
                return Ok(countries);
            }
        }
    }
}
