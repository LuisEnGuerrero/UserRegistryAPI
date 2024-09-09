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

        public async Task CreateUserAsync(User user)
        {
            // Validar existencia de entidades
            if (!await _userRepository.CountryExistsAsync(user.CountryId))
                throw new ArgumentException("El país no existe.");

            if (!await _userRepository.DepartmentExistsAsync(user.DepartmentId))
                throw new ArgumentException("El departamento no existe.");

            if (!await _userRepository.MunicipalityExistsAsync(user.MunicipalityId))
                throw new ArgumentException("El municipio no existe.");

            // Llamar al repositorio para crear el usuario
            await _userRepository.CreateUserAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }


    }
}
