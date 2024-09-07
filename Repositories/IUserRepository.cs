using UserRegistryAPI.Models;

namespace UserRegistryAPI.Repositories
{
    public interface IUserRepository
    {
        Task CreateUserAsync(User user);
        Task<bool> CountryExistsAsync(int countryId);
        Task<bool> DepartmentExistsAsync(int departmentId);
        Task<bool> MunicipalityExistsAsync(int municipalityId);
    }
}
