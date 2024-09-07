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
                    user.Name,
                    user.Phone,
                    user.Address,
                    user.CountryId,
                    user.DepartmentId,
                    user.MunicipalityId
                };

                await connection.ExecuteAsync("CALL sp_create_user(@Nombre, @Telefono, @Direccion, @PaisId, @DepartamentoId, @MunicipioId)", parameters);
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
