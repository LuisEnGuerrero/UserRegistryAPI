using Dapper;
using Google.Apis.Auth;
using UserRegistryAPI.Data;
using System.Data;
using System.Threading.Tasks;

public class AuthUserRepository : IAuthUserRepository
{
    private readonly DatabaseConfig _databaseConfig;

    public AuthUserRepository(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    // Obtener un usuario por email
    public async Task<AuthUser?> GetByEmailAsync(string email)
    {
        using (var connection = _databaseConfig.CreateConnection())
        {
            try
            {
                return await connection.QuerySingleOrDefaultAsync<AuthUser>(
                    "SELECT * FROM auth_users WHERE email = @Email", new { Email = email });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario por email: {ex.Message}");
                return null;
            }
        }
    }

    // Obtener un usuario por GoogleId
    public async Task<AuthUser?> GetByGoogleIdAsync(string googleId)
    {
        using (var connection = _databaseConfig.CreateConnection())
        {
            try
            {
                return await connection.QuerySingleOrDefaultAsync<AuthUser>(
                    "SELECT * FROM auth_users WHERE google_id = @GoogleId", new { GoogleId = googleId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario por GoogleId: {ex.Message}");
                return null;
            }
        }
    }

    // Validar token de Google
    public async Task<AuthUser?> ValidateGoogleTokenAsync(string token)
    {
        try
        {
            // Validar el token utilizando el API de Google
            var payload = await GoogleJsonWebSignature.ValidateAsync(token);
            if (payload == null)
            {
                return null;
            }

            return new AuthUser
            {
                GoogleId = payload.Subject,
                Username = payload.Name,
                Email = payload.Email
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al validar token de Google: {ex.Message}");
            return null;
        }
    }

    // Crear un usuario-roll
    public async Task CreateAuthUserAsync(AuthUser authUser)
    {
        using (var connection = _databaseConfig.CreateConnection())
        {
            try
            {
                await connection.ExecuteAsync(
                    "INSERT INTO auth_users (username, email, password_hash, role, is_google_auth, google_id) " +
                    "VALUES (@Username, @Email, @PasswordHash, @Role, @IsGoogleAuth, @GoogleId)", authUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
            }
        }
    }

    // Obtener un usuario por ID
    public async Task<AuthUser?> GetByIdAsync(int id)
    {
        using (var connection = _databaseConfig.CreateConnection())
        {
            try
            {
                return await connection.QuerySingleOrDefaultAsync<AuthUser>(
                    "SELECT * FROM auth_users WHERE id = @Id", new { Id = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario por ID: {ex.Message}");
                return null;
            }
        }
    }

    // Obtener todos los usuarios
    public async Task<IEnumerable<AuthUser>> GetAllUsersAsync()
    {
        using (var connection = _databaseConfig.CreateConnection())
        {
            try
            {
                return await connection.QueryAsync<AuthUser>("SELECT * FROM auth_users");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return Enumerable.Empty<AuthUser>();
            }
        }
    }

    // Actualizar un usuario
    public async Task UpdateUserAsync(AuthUser authUser)
    {
        using (var connection = _databaseConfig.CreateConnection())
        {
            try
            {
                await connection.ExecuteAsync(
                    "UPDATE auth_users SET username = @Username, email = @Email, password_hash = @PasswordHash, role = @Role, is_google_auth = @IsGoogleAuth, google_id = @GoogleId " +
                    "WHERE id = @Id", authUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
            }
        }
    }

    // Eliminar un usuario
    public async Task DeleteUserAsync(int id)
    {
        using (var connection = _databaseConfig.CreateConnection())
        {
            try
            {
                await connection.ExecuteAsync(
                    "DELETE FROM auth_users WHERE id = @Id", new { Id = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
            }
        }
    }
}
