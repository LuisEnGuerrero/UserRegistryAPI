using Npgsql;
using UserRegistryAPI.Models;


namespace UserRegistryAPI.Repositories
{
    public class ListRepository
    {
        private string _connectionString = "your_connection_string_here";

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("CALL sp_get_all_users()", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("nombre")),
                                Phone = reader.GetString(reader.GetOrdinal("telefono")),
                                Address = reader.GetString(reader.GetOrdinal("direccion")),
                                CountryId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("departamento_id")),
                                MunicipalityId = reader.GetInt32(reader.GetOrdinal("municipio_id"))
                            });
                        }
                    }
                }
            }
            return users;
        }
    }
}
