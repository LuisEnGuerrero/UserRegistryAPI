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

        public UserRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task CreateUserAsync(User user)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                var parameters = new
                {
                    Nombre = user.Name,          // "Nombre" debe coincidir con el parámetro del stored procedure
                    Telefono = user.Phone,
                    Direccion = user.Address,
                    PaisId = user.CountryId,
                    DepartamentoId = user.DepartmentId,
                    MunicipioId = user.MunicipalityId
                };

                // Agregamos el log para verificar los parámetros antes de la ejecución
                Console.WriteLine($"Nombre: {user.Name}, Telefono: {user.Phone}, Direccion: {user.Address}, PaisId: {user.CountryId}, DepartamentoId: {user.DepartmentId}, MunicipioId: {user.MunicipalityId}");

                // Llamada a Dapper para ejecutar el procedimiento almacenado
                await connection.ExecuteAsync("CALL sp_create_user(@Nombre, @Telefono, @Direccion, @PaisId, @DepartamentoId, @MunicipioId)", parameters);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                return await connection.QueryAsync<User>(
                     "SELECT id, nombre AS name, telefono AS phone, direccion AS address, pais_id AS countryId, departamento_id AS departmentId, municipio_id AS municipalityId FROM usuario;"
                );
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                var parameters = new { p_id = id };
                var result = await connection.QuerySingleOrDefaultAsync<User>(
                        "SELECT id, nombre AS name, telefono AS phone, direccion AS address, pais_id AS countryId, departamento_id AS departmentId, municipio_id AS municipalityId FROM usuario WHERE id = @p_id;",
                        parameters
                    );
                return result ?? default;
            }
        }

        public async Task UpdateUserAsync(User user)
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

                await connection.ExecuteAsync("SELECT sp_update_user(@p_id, @p_nombre, @p_telefono, @p_direccion, @p_pais_id, @p_departamento_id, @p_municipio_id);", parameters);
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                var parameters = new { p_id = id };
                await connection.ExecuteAsync("SELECT sp_delete_user(@p_id);", parameters);
            }
        }


        public async Task<bool> CountryExistsAsync(int countryId)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM pais WHERE id = @Id", new { Id = countryId });
            }
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM departamento WHERE id = @Id", new { Id = departmentId });
            }
        }

        public async Task<bool> MunicipalityExistsAsync(int municipalityId)
        {
            using (var connection = _databaseConfig.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM municipio WHERE id = @Id", new { Id = municipalityId });
            }
        }
    }
}
