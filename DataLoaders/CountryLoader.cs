using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using UserRegistryAPI.Data;
using UserRegistryAPI.Models;
using System.Collections.Generic;

public class CountryLoader
{
    private readonly DatabaseConfig _databaseConfig;

    public CountryLoader(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    public async Task LoadCountriesAsync(string filePath)
    {
        try
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                var countries = ReadCountriesFromCsv(filePath);

                foreach (var country in countries)
                {
                    var exists = await connection.ExecuteScalarAsync<bool>(
                        "SELECT COUNT(1) FROM pais WHERE nombre = @Name",
                        new { Name = country });

                    if (!exists)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO pais (nombre) VALUES (@Name)",
                            new { Name = country });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones: loguear el error o manejarlo según sea necesario
            Console.WriteLine($"Error al cargar los países desde el archivo CSV: {ex.Message}");
        }
    }

    private IEnumerable<string> ReadCountriesFromCsv(string filePath)
    {
        try
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                // Asegúrate de usar GetRecords<string> para leer los registros como una lista de cadenas
                var records = csv.GetRecords<string>().ToList();
                return records;
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones: loguear el error o manejarlo según sea necesario
            Console.WriteLine($"Error al leer el archivo CSV: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }
}
