using UserRegistryAPI.Models;

namespace UserRegistryAPI.Repositories
{
    public interface IUserRepository
    {
        // Método para crear un nuevo usuario
        Task CreateUserAsync(User user);

        // Métodos para verificar la existencia de país, departamento y municipalidad
        Task<bool> CountryExistsAsync(int countryId);
        Task<bool> DepartmentExistsAsync(int departmentId);
        Task<bool> MunicipalityExistsAsync(int municipalityId);

        // Métodos para leer usuarios (Read)
        Task<IEnumerable<User>> GetAllUsersAsync();  // Obtener todos los usuarios
        Task<User> GetUserByIdAsync(int id);         // Obtener un usuario por su ID

        // Método para actualizar un usuario (Update)
        Task UpdateUserAsync(User user);

        // Método para eliminar un usuario (Delete)
        Task DeleteUserAsync(int id);
 
    }
}
