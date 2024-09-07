using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly Data.DatabaseConfig _databaseConfig;

        public DepartmentsController(Data.DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            using (IDbConnection dbConnection = _databaseConfig.CreateConnection())
            {
                var query = "SELECT * FROM departamento";
                var departments = dbConnection.Query(query);
                return Ok(departments);
            }
        }
    }
}
