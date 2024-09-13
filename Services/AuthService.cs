using Google.Apis.Auth;
using UserRegistryAPI.Models;
using System.Threading.Tasks;
using BCrypt.Net;

public class AuthService
{
    private readonly IAuthUserRepository _authUserRepository;
    private readonly JwtService _jwtService;

    public AuthService(IAuthUserRepository authUserRepository, JwtService jwtService)
    {
        _authUserRepository = authUserRepository;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Registra un nuevo usuario con rol y encripta su contraseña.
    /// </summary>
    public async Task<AuthUser> RegisterUserAsync(AuthUser user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password); // Encriptar la contraseña

        await _authUserRepository.CreateAuthUserAsync(user); // Guardar el usuario
        return user;
    }

    /// <summary>
    /// Autentica un usuario por email y contraseña, y genera un JWT.
    /// </summary>
    public async Task<string?> AuthenticateUserAsync(string email, string password)
    {
        var user = await ValidateLoginAsync(email, password);
        if (user == null)
            return null;

        return GenerateJwtToken(user);
    }

    /// <summary>
    /// Autentica un usuario con Google, valida el token de Google, y genera un JWT.
    /// </summary>
    public async Task<string?> AuthenticateGoogleUserAsync(string googleToken)
    {
        var user = await ValidateGoogleLoginAsync(googleToken);
        if (user == null)
            return null;

        return GenerateJwtToken(user);
    }

    /// <summary>
    /// Genera un token JWT para el usuario autenticado.
    /// </summary>
    public string GenerateJwtToken(AuthUser user)
    {
        return _jwtService.GenerateJwtToken(user);
    }

    /// <summary>
    /// Valida el login de un usuario por email y contraseña, asegurándose de que esté autorizado.
    /// </summary>
    public async Task<AuthUser?> ValidateLoginAsync(string email, string password)
    {
        var user = await _authUserRepository.GetByEmailAsync(email);

        // Validar existencia de usuario, contraseña y autorización
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) || !user.IsAuthorized)
            return null;

        return user;
    }

    /// <summary>
    /// Maneja el login mediante Google y crea un usuario si no existe.
    /// </summary>
    public async Task<AuthUser?> GoogleLoginAsync(GoogleLoginRequest request)
    {
        var googleUser = await _authUserRepository.ValidateGoogleTokenAsync(request.Token);
        if (googleUser == null)
            return null;

        // Verificar si el usuario ya existe
        var user = await _authUserRepository.GetByGoogleIdAsync(googleUser.GoogleId ?? "");

        if (user == null)
        {
            user = new AuthUser
            {
                Username = googleUser.Username,
                Email = googleUser.Email,
                GoogleId = googleUser.GoogleId,
                IsGoogleAuth = true,
                Role = "Viewer",  // Rol por defecto
                IsAuthorized = false  // Aprobación pendiente por AdminMaster
            };

            await _authUserRepository.CreateAuthUserAsync(user);
            return null; // No autorizado aún
        }

        return user.IsAuthorized ? user : null;
    }

    /// <summary>
    /// Valida el login mediante Google y asegura que el usuario esté autorizado.
    /// </summary>
    public async Task<AuthUser?> ValidateGoogleLoginAsync(string googleToken)
    {
        var user = await _authUserRepository.GetByGoogleIdAsync(googleToken);
        return user != null && user.IsAuthorized ? user : null;
    }

    /// <summary>
    /// Obtiene un usuario por email.
    /// </summary>
    public async Task<AuthUser?> GetByEmailAsync(string email)
    {
        return await _authUserRepository.GetByEmailAsync(email);
    }

    /// <summary>
    /// Crea un nuevo usuario con rol.
    /// </summary>
    public async Task CreateAuthUserAsync(AuthUser user)
    {
        await _authUserRepository.CreateAuthUserAsync(user);
    }

    /// <summary>
    /// Obtiene todos los usuarios con rol.
    /// </summary>
    public async Task<IEnumerable<AuthUser>> GetAllUsersAsync()
    {
        return await _authUserRepository.GetAllUsersAsync();
    }

    /// <summary>
    /// Obtiene un usuario por ID.
    /// </summary>
    public async Task<AuthUser?> GetByIdAsync(int id)
    {
        return await _authUserRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Actualiza un usuario con rol.
    /// </summary>
    public async Task UpdateUserAsync(AuthUser user)
    {
        await _authUserRepository.UpdateUserAsync(user);
    }

    /// <summary>
    /// Elimina un usuario con rol.
    /// </summary>
    public async Task DeleteUserAsync(int id)
    {
        await _authUserRepository.DeleteUserAsync(id);
    }
}
