using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserRegistryAPI.Data;
using UserRegistryAPI.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataLoadController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly string _securityKey = "vFUEHrFQdPlvE1mCpn9I5LMKwzBdNC3BJuUafsC1LC4"; // Key definida

        public DataLoadController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("cargardatos")]
        public IActionResult LoadData([FromQuery] string key)
        {
            // Validar la clave de seguridad
            if (key != _securityKey)
            {
                return Unauthorized("Clave de seguridad inválida.");
            }

            var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");

            if (!Directory.Exists(dataDirectory))
            {
                return NotFound("La carpeta de datos no existe.");
            }

            var files = Directory.GetFiles(dataDirectory, "*.csv");

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file).ToLower();

                switch (fileName)
                {
                    case "pais":
                        LoadCountriesData(file);
                        break;
                    case "departamentos":
                        LoadDepartmentsData(file);
                        break;
                    case "municipios":
                        LoadCitiesData(file);
                        break;
                    case "usuarios":
                        LoadUsersData(file);
                        break;
                    default:
                        break;
                }
            }

            _context.SaveChanges();

            return Ok("Datos cargados exitosamente.");
        }

        private void LoadCountriesData(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true, // Configura esto en false si el archivo no tiene encabezado
                    IgnoreBlankLines = true,
                    BadDataFound = null // Ignorar datos corruptos
                };

                using var csv = new CsvReader(reader, csvConfig);

                // Lee los registros como cadenas si no hay encabezado
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    // El registro se trata como un diccionario de claves y valores
                    var recordDict = record as IDictionary<string, object>;
                    var countryName = recordDict?.FirstOrDefault().Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(countryName))
                    {
                        if (!_context.Countries.Any(c => c.Name == countryName))
                        {
                            _context.Countries.Add(new Country { Name = countryName });
                        }
                    }
                }

                _context.SaveChanges(); // Guarda cambios aquí, si es necesario
            }
            catch (Exception ex)
            {
                // Registrar y mostrar el error para depuración
                Console.WriteLine($"Error al cargar datos de países: {ex.Message}");
                throw;
            }

        }

        private void LoadDepartmentsData(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = false // Cambiar a true si tu CSV tiene encabezados
                };

                using var csv = new CsvReader(reader, csvConfig);

                // Leer los registros del CSV
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    // Extraer los valores del CSV
                    var recordDict = record as IDictionary<string, object>;
                    var departmentName = recordDict.Values.ElementAtOrDefault(0)?.ToString() ?? string.Empty;
                    var countryId = int.Parse(recordDict.Values.ElementAt(1)?.ToString() ?? "0");

                    if (!string.IsNullOrWhiteSpace(departmentName) && countryId > 0)
                    {
                        // Verifica si ya existe un departamento con el mismo nombre y CountryId
                        if (!_context.Departments.Any(d => d.Name == departmentName && d.CountryId == countryId))
                        {
                            _context.Departments.Add(new Department
                            {
                                Name = departmentName,  // Mapea el nombre a la propiedad Name
                                CountryId = countryId   // Mapea pais_id a la propiedad CountryId
                            });
                        }
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Registrar y mostrar el error para depuración
                Console.WriteLine($"Error al cargar datos de departamentos: {ex.Message}");
                throw;
            }
        }


        private void LoadCitiesData(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true // Cambiar a true si tu CSV tiene encabezados
                };

                using var csv = new CsvReader(reader, csvConfig);
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    // Extraer los valores del CSV
                    var recordDict = record as IDictionary<string, object>;
                    var cityName = recordDict.Values.ElementAtOrDefault(0)?.ToString() ?? string.Empty;
                    var departmentId = int.Parse(recordDict.Values.ElementAt(1)?.ToString() ?? "0");

                    if (!string.IsNullOrWhiteSpace(cityName) && departmentId > 0)
                    {
                        // Verifica si ya existe un municipio con el mismo nombre y DepartmentId
                        if (!_context.Municipalities.Any(c => c.Name == cityName && c.DepartmentId == departmentId))
                        {
                            _context.Municipalities.Add(new Municipality
                            {
                                Name = cityName,     // Mapea el nombre a la propiedad Name
                                DepartmentId = departmentId   // Mapea department_id a la propiedad DepartmentId
                            });
                        }
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Registrar y mostrar el error para depuración
                Console.WriteLine($"Error al cargar datos de municipios: {ex.Message}");
                throw;
            }
        }

        private void LoadUsersData(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            };
            using var csv = new CsvReader(reader, csvConfig);
            var records = csv.GetRecords<User>().ToList();
            foreach (var record in records)
            {
                if (!_context.Users.Any(u => u.Name == record.Name && u.Phone == record.Phone))
                {
                    _context.Users.Add(record);
                }
            }
        }
    }
}
