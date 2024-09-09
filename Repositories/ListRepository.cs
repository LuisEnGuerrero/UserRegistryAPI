using Npgsql;
using UserRegistryAPI.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace UserRegistryAPI.Repositories
{
    public class ListRepository
    {
        private readonly string _connectionString;

        // El constructor recibe la configuración y extrae la cadena de conexión
        public ListRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("La cadena de conexión no puede ser nula.");
        }

        /// <summary>
        /// Obtiene todos los usuarios llamando a un procedimiento almacenado.
        /// </summary>
        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            try
            {
                // Establecer conexión con PostgreSQL
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Llamar al procedimiento almacenado
                    using (var cmd = new NpgsqlCommand("CALL sp_get_all_users()", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Leer los resultados y mapearlos a la lista de usuarios
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("nombre")),
                                    Phone = reader.GetString(reader.GetOrdinal("telefono")),
                                    Address = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion")),
                                    CountryId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("departamento_id")),
                                    MunicipalityId = reader.GetInt32(reader.GetOrdinal("municipio_id"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo básico de excepciones. Aquí se podría agregar un logging para seguimiento.
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                // Opcional: lanzar la excepción nuevamente o manejarla según sea necesario
            }

            return users;
        }
    }
}
