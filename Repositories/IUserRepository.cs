using UserRegistryAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserRegistryAPI.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de usuarios.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        Task CreateUserAsync(User user);

        /// <summary>
        /// Verifica si un país existe.
        /// </summary>
        Task<bool> CountryExistsAsync(int countryId);

        /// <summary>
        /// Verifica si un departamento existe.
        /// </summary>
        Task<bool> DepartmentExistsAsync(int departmentId);

        /// <summary>
        /// Verifica si un municipio existe.
        /// </summary>
        Task<bool> MunicipalityExistsAsync(int municipalityId);

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Obtiene un usuario por ID.
        /// </summary>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Elimina un usuario por ID.
        /// </summary>
        Task DeleteUserAsync(int id);
    }
}
