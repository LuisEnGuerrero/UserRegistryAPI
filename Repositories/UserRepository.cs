using Npgsql;
using System.Data;
using Dapper;
using UserRegistryAPI.Models;
using UserRegistryAPI.Data;

namespace UserRegistryAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        // El constructor inyecta la configuración de la base de datos
        public UserRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        /// <summary>
        /// Crea un nuevo usuario llamando a un procedimiento almacenado.
        /// </summary>
        public async Task CreateUserAsync(User user)
        {
            try
            {
                using (var connection = _databaseConfig.CreateConnection())
                {
                    var parameters = new
                    {
                        Nombre = user.Name,
                        Telefono = user.Phone,
                        Direccion = user.Address,
                        PaisId = user.CountryId,
                        DepartamentoId = user.DepartmentId,
                        MunicipioId = user.MunicipalityId
                    };

                    // Log de parámetros para verificación (opcional para debugging)
                    Console.WriteLine($"Creando usuario: {user.Name}, Teléfono: {user.Phone}");

                    // Llama al procedimiento almacenado para crear el usuario
                    await connection.ExecuteAsync("CALL sp_create_user(@Nombre, @Telefono, @Direccion, @PaisId, @DepartamentoId, @MunicipioId)", parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios de la base de datos.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                using (var connection = _databaseConfig.CreateConnection())
                {
                    return await connection.QueryAsync<User>(
                        "SELECT id, nombre AS Name, telefono AS Phone, direccion AS Address, pais_id AS CountryId, departamento_id AS DepartmentId, municipio_id AS MunicipalityId FROM usuario;"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return Enumerable.Empty<User>();
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                using (var connection = _databaseConfig.CreateConnection())
                {
                    var parameters = new { p_id = id };
                    return await connection.QuerySingleOrDefaultAsync<User>(
                        "SELECT id, nombre AS Name, telefono AS Phone, direccion AS Address, pais_id AS CountryId, departamento_id AS DepartmentId, municipio_id AS MunicipalityId FROM usuario WHERE id = @p_id;",
                        parameters
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el usuario por ID: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            try
            {
                using (var connection = _databaseConfig.CreateConnection())
                {
                    var parameters = new
                    {
                        p_id = user.Id,
                        p_nombre = user.Name,
                        p_telefono = user.Phone,
                        p_direccion = user.Address,
                        p_pais_id = user.CountryId,
                        p_departamento_id = user.DepartmentId,
                        p_municipio_id = user.MunicipalityId
                    };

                    // Llama al procedimiento almacenado para actualizar el usuario
                    await connection.ExecuteAsync("CALL sp_update_user(@p_id, @p_nombre, @p_telefono, @p_direccion, @p_pais_id, @p_departamento_id, @p_municipio_id);", parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un usuario por su ID.
        /// </summary>
        public async Task DeleteUserAsync(int id)
        {
            try
            {
                using (var connection = _databaseConfig.CreateConnection())
                {
                    var parameters = new { p_id = id };
                    await connection.ExecuteAsync("CALL sp_delete_user(@p_id);", parameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si un país existe.
        /// </summary>
        public async Task<bool> CountryExistsAsync(int countryId)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM pais WHERE id = @Id", new { Id = countryId });
            }
        }

        /// <summary>
        /// Verifica si un departamento existe.
        /// </summary>
        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM departamento WHERE id = @Id", new { Id = departmentId });
            }
        }

        /// <summary>
        /// Verifica si un municipio existe.
        /// </summary>
        public async Task<bool> MunicipalityExistsAsync(int municipalityId)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM municipio WHERE id = @Id", new { Id = municipalityId });
            }
        }
    }
}
