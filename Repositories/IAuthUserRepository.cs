using System.Threading.Tasks;
using UserRegistryAPI.Models;

/// <summary>
/// Interfaz para la gestión de los usuarios autenticados (AuthUser),
/// incluyendo usuarios registrados mediante Google o autenticación tradicional.
/// </summary>
public interface IAuthUserRepository
{
    /// <summary>
    /// Obtiene un usuario autenticado (AuthUser) mediante su correo electrónico.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario.</param>
    /// <returns>Devuelve el usuario autenticado o null si no existe.</returns>
    Task<AuthUser?> GetByEmailAsync(string email);

    /// <summary>
    /// Obtiene un usuario autenticado (AuthUser) mediante su GoogleId.
    /// </summary>
    /// <param name="googleId">Identificador único de Google (GoogleId).</param>
    /// <returns>Devuelve el usuario autenticado o null si no existe.</returns>
    Task<AuthUser?> GetByGoogleIdAsync(string googleId);

    /// <summary>
    /// Crea un nuevo usuario autenticado (AuthUser) en la base de datos.
    /// </summary>
    /// <param name="authUser">Objeto AuthUser con la información del nuevo usuario.</param>
    Task CreateAuthUserAsync(AuthUser authUser);

    /// <summary>
    /// Obtiene un usuario autenticado (AuthUser) por su ID único.
    /// </summary>
    /// <param name="id">Identificador único del usuario (ID).</param>
    /// <returns>Devuelve el usuario autenticado o null si no existe.</returns>
    Task<AuthUser?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene una lista de todos los usuarios autenticados (AuthUser) en la base de datos.
    /// </summary>
    /// <returns>Devuelve una lista de usuarios autenticados.</returns>
    Task<IEnumerable<AuthUser>> GetAllUsersAsync(); // Obtener todos los usuarios-roll

    /// <summary>
    /// Actualiza la información de un usuario autenticado (AuthUser) existente.
    /// </summary>
    /// <param name="authUser">Objeto AuthUser con la información actualizada del usuario.</param>
    Task UpdateUserAsync(AuthUser authUser);        // Actualizar un usuario-roll

    /// <summary>
    /// Elimina un usuario autenticado (AuthUser) por su ID.
    /// </summary>
    /// <param name="id">Identificador único del usuario (ID).</param>
    Task DeleteUserAsync(int id);                   // Eliminar un usuario-roll

    /// <summary>
    /// Valida el token de Google para la autenticación del usuario.
    /// </summary>
    /// <param name="token">Token de Google para validar la identidad del usuario.</param>
    /// <returns>Devuelve el usuario autenticado con Google o null si el token es inválido.</returns>
    Task<AuthUser?> ValidateGoogleTokenAsync(string token); // Validar el token de Google
}
