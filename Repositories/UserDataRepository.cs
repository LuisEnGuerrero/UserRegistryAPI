using Npgsql;
using UserRegistryAPI.Models;
using System;
using Microsoft.Extensions.Configuration;

public class UserDataRepository
{
    private readonly string _connectionString;

    // El constructor obtiene la cadena de conexión desde la configuración
    public UserDataRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("La cadena de conexión no puede ser nula.");
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    public User? GetUserById(int id)
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Llama al procedimiento almacenado para obtener un usuario por su ID
                using (var cmd = new NpgsqlCommand("CALL sp_get_user_by_id(@p_id)", connection))
                {
                    cmd.Parameters.AddWithValue("p_id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("p_nombre")),
                                Phone = reader.GetString(reader.GetOrdinal("p_telefono")),
                                Address = reader.IsDBNull(reader.GetOrdinal("p_direccion")) ? null : reader.GetString(reader.GetOrdinal("p_direccion")),
                                CountryId = reader.GetInt32(reader.GetOrdinal("p_pais_id")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("p_departamento_id")),
                                MunicipalityId = reader.GetInt32(reader.GetOrdinal("p_municipio_id"))
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Loguear el error o manejarlo según sea necesario
            Console.WriteLine($"Error al obtener el usuario por ID: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Actualiza un usuario existente.
    /// </summary>
    public void UpdateUser(User user)
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Llama al procedimiento almacenado para actualizar un usuario
                using (var cmd = new NpgsqlCommand("CALL sp_update_user(@p_id, @p_nombre, @p_telefono, @p_direccion, @p_pais_id, @p_departamento_id, @p_municipio_id)", connection))
                {
                    cmd.Parameters.AddWithValue("p_id", user.Id);
                    cmd.Parameters.AddWithValue("p_nombre", user.Name);
                    cmd.Parameters.AddWithValue("p_telefono", user.Phone);
                    cmd.Parameters.AddWithValue("p_direccion", user.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_pais_id", user.CountryId);
                    cmd.Parameters.AddWithValue("p_departamento_id", user.DepartmentId);
                    cmd.Parameters.AddWithValue("p_municipio_id", user.MunicipalityId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Loguear el error o manejarlo según sea necesario
            Console.WriteLine($"Error al actualizar el usuario: {ex.Message}");
        }
    }

    /// <summary>
    /// Elimina un usuario por su ID.
    /// </summary>
    public void DeleteUser(int id)
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Llama al procedimiento almacenado para eliminar un usuario
                using (var cmd = new NpgsqlCommand("CALL sp_delete_user(@p_id)", connection))
                {
                    cmd.Parameters.AddWithValue("p_id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Loguear el error o manejarlo según sea necesario
            Console.WriteLine($"Error al eliminar el usuario: {ex.Message}");
        }
    }
}
