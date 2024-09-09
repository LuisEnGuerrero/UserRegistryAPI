using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserRegistryAPI.Data;
using UserRegistryAPI.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace UserRegistryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataLoadController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly string _securityKey;

        // Constructor que inyecta el contexto de base de datos y la clave de seguridad desde la configuración
        public DataLoadController(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _securityKey = configuration["KEY_SEND_DATA_CSV"] ?? throw new ArgumentNullException("KEY_SEND_DATA_CSV no encontrada en las variables de entorno.");
        }

        /// <summary>
        /// Método GET para cargar datos desde archivos CSV. Valida la clave de seguridad antes de proceder.
        /// </summary>
        /// <param name="key">Clave de seguridad para la carga de datos</param>
        /// <returns>Resultado de la carga de datos</returns>
        [HttpGet("cargardatos")]
        public IActionResult LoadData([FromQuery] string key)
        {
            // Validar la clave de seguridad proporcionada
            if (key != _securityKey)
            {
                return Unauthorized("Clave de seguridad inválida.");
            }

            // Verificar si la carpeta de datos existe
            var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");

            if (!Directory.Exists(dataDirectory))
            {
                return NotFound("La carpeta de datos no existe.");
            }

            // Obtener todos los archivos CSV en la carpeta de datos
            var files = Directory.GetFiles(dataDirectory, "*.csv");

            if (files.Length == 0)
            {
                return NotFound("No se encontraron archivos CSV para cargar.");
            }

            try
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file).ToLower();

                    // Procesar cada archivo CSV según su nombre
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
                            // Archivo desconocido, no se procesa
                            break;
                    }
                }

                // Guardar los cambios en la base de datos después de cargar todos los archivos
                _context.SaveChanges();

                return Ok("Datos cargados exitosamente.");
            }
            catch (Exception ex)
            {
                // Devolver un error genérico si algo sale mal durante la carga de los datos
                return StatusCode(500, $"Ocurrió un error al cargar los datos: {ex.Message}");
            }
        }

        /// <summary>
        /// Cargar datos de países desde un archivo CSV.
        /// </summary>
        /// <param name="filePath">Ruta del archivo CSV</param>
        private void LoadCountriesData(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    BadDataFound = null // Ignorar datos corruptos
                };

                using var csv = new CsvReader(reader, csvConfig);
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    var recordDict = record as IDictionary<string, object>;
                    var countryName = recordDict?.FirstOrDefault().Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(countryName))
                    {
                        // Verificar si el país ya existe antes de agregarlo
                        if (!_context.Countries.Any(c => c.Name == countryName))
                        {
                            _context.Countries.Add(new Country { Name = countryName });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear y lanzar la excepción para ser manejada en el método principal
                Console.WriteLine($"Error al cargar datos de países: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cargar datos de departamentos desde un archivo CSV.
        /// </summary>
        /// <param name="filePath">Ruta del archivo CSV</param>
        private void LoadDepartmentsData(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = false // Cambiar a true si el CSV tiene encabezados
                };

                using var csv = new CsvReader(reader, csvConfig);
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    var recordDict = record as IDictionary<string, object>;

                    if (recordDict != null)
                    {
                        var departmentValue = recordDict.Values.ElementAtOrDefault(0);
                        var departmentName = departmentValue != null ? departmentValue.ToString() : string.Empty;

                        var countryIdValue = recordDict.Values.ElementAtOrDefault(1);
                        var countryId = countryIdValue != null && int.TryParse(countryIdValue.ToString(), out int parsedCountryId) ? parsedCountryId : 0;

                        // Verificar si el nombre del departamento y countryId son válidos antes de agregarlo
                        if (!string.IsNullOrWhiteSpace(departmentName) && countryId > 0)
                        {
                            if (!_context.Departments.Any(d => d.Name == departmentName && d.CountryId == countryId))
                            {
                                _context.Departments.Add(new Department
                                {
                                    Name = departmentName,
                                    CountryId = countryId
                                });
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos de departamentos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cargar datos de municipios desde un archivo CSV.
        /// </summary>
        /// <param name="filePath">Ruta del archivo CSV</param>
        private void LoadCitiesData(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true
                };

                using var csv = new CsvReader(reader, csvConfig);
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    // Verificar si 'record' se puede convertir a un diccionario
                    var recordDict = record as IDictionary<string, object>;

                    // Verificar si 'recordDict' no es nulo antes de acceder a sus valores
                    if (recordDict != null)
                    {
                        // Obtener el nombre de la ciudad (índice 0) y manejar posibles valores nulos
                        var cityNameValue = recordDict.Values.ElementAtOrDefault(0);
                        var cityName = cityNameValue != null ? cityNameValue.ToString() : string.Empty;

                        // Obtener el ID del departamento (índice 1) y manejar posibles valores nulos
                        var departmentIdValue = recordDict.Values.ElementAtOrDefault(1);
                        var departmentId = departmentIdValue != null && int.TryParse(departmentIdValue.ToString(), out int parsedDepartmentId) ? parsedDepartmentId : 0;

                        // Solo proceder si el nombre de la ciudad no está vacío y el departamento es válido
                        if (!string.IsNullOrWhiteSpace(cityName) && departmentId > 0)
                        {
                            // Verificar si el municipio no existe ya en la base de datos antes de agregarlo
                            if (!_context.Municipalities.Any(c => c.Name == cityName && c.DepartmentId == departmentId))
                            {
                                _context.Municipalities.Add(new Municipality
                                {
                                    Name = cityName,
                                    DepartmentId = departmentId
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos de municipios: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cargar datos de usuarios desde un archivo CSV.
        /// </summary>
        /// <param name="filePath">Ruta del archivo CSV</param>
        private void LoadUsersData(string filePath)
        {
            try
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

                _context.SaveChanges(); // Guardar los usuarios después de procesar el archivo
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos de usuarios: {ex.Message}");
                throw;
            }
        }
    }
}
