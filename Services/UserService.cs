using UserRegistryAPI.Models;
using UserRegistryAPI.Repositories;

namespace UserRegistryAPI.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Crea un nuevo usuario después de validar la existencia de las entidades relacionadas.
        /// </summary>
        public async Task CreateUserAsync(User user)
        {
            if (!await _userRepository.CountryExistsAsync(user.CountryId))
                throw new ArgumentException("El país no existe.");

            if (!await _userRepository.DepartmentExistsAsync(user.DepartmentId))
                throw new ArgumentException("El departamento no existe.");

            if (!await _userRepository.MunicipalityExistsAsync(user.MunicipalityId))
                throw new ArgumentException("El municipio no existe.");

            await _userRepository.CreateUserAsync(user);
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Actualiza un usuario existente después de validar que el usuario existe.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(user.Id);
            if (existingUser == null)
                throw new ArgumentException("El usuario no existe.");

            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Elimina un usuario por su ID después de validar que el usuario existe.
        /// </summary>
        public async Task DeleteUserAsync(int id)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                throw new ArgumentException("El usuario no existe.");

            await _userRepository.DeleteUserAsync(id);
        }
    }
}
