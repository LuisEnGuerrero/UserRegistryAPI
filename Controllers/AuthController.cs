using Microsoft.AspNetCore.Mvc;
using UserRegistryAPI.Models;
using UserRegistryAPI.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthUserController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthUserController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login regular de un usuario con email y contraseña.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.ValidateLoginAsync(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized("Credenciales inválidas.");
        }

        var token = _authService.GenerateJwtToken(user);  // Generar el JWT
        return Ok(new { Token = token });
    }

    /// <summary>
    /// Crear un usuario con rol. Solo AdminMaster puede realizar esta acción.
    /// </summary>
    [Authorize(Roles = "AdminMaster")]
    [HttpPost("create-auth-user")]
    public async Task<IActionResult> CreateAuthUser([FromBody] AuthUser request)
    {
        var newUser = new AuthUser
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash), // Hashear la contraseña
            Role = request.Role
        };

        await _authService.CreateAuthUserAsync(newUser);
        return Ok("Usuario con rol creado exitosamente.");
    }

    /// <summary>
    /// Obtener todos los usuarios con rol.
    /// Disponible para AdminMaster, Viewer, CreatorAdmin, EditorAdmin.
    /// </summary>
    [Authorize(Roles = "AdminMaster, Viewer, CreatorAdmin, EditorAdmin")]
    [HttpGet("get-auth-users")]
    public async Task<IActionResult> GetAuthUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Eliminar un usuario con rol. Solo AdminMaster puede eliminar usuarios.
    /// </summary>
    [Authorize(Roles = "AdminMaster")]
    [HttpDelete("delete-auth-user/{id}")]
    public async Task<IActionResult> DeleteAuthUser(int id)
    {
        var user = await _authService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        await _authService.DeleteUserAsync(id);
        return Ok("Usuario con rol eliminado exitosamente.");
    }

    /// <summary>
    /// Actualizar un usuario con rol. Disponible para AdminMaster y EditorAdmin.
    /// </summary>
    [Authorize(Roles = "AdminMaster, EditorAdmin")]
    [HttpPut("update-auth-user/{id}")]
    public async Task<IActionResult> UpdateAuthUser(int id, [FromBody] AuthUser request)
    {
        var existingUser = await _authService.GetByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        // Solo AdminMaster puede modificar usuarios con rol AdminMaster
        if (User.IsInRole("AdminMaster") || (User.IsInRole("EditorAdmin") && existingUser.Role != "AdminMaster"))
        {
            existingUser.Username = request.Username;
            existingUser.Email = request.Email;
            existingUser.Role = request.Role;

            await _authService.UpdateUserAsync(existingUser);
            return Ok("Usuario con rol actualizado exitosamente.");
        }

        return Forbid("No tienes permiso para actualizar este tipo de usuario.");
    }

    /// <summary>
    /// Autorizar a un usuario para que pueda hacer login.
    /// Solo AdminMaster puede autorizar.
    /// </summary>
    [Authorize(Roles = "AdminMaster")]
    [HttpPut("authorize-login/{id}")]
    public async Task<IActionResult> AuthorizeLogin(int id)
    {
        var user = await _authService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        user.IsAuthorized = true; // Autorizar al usuario
        await _authService.UpdateUserAsync(user);
        return Ok("Usuario autorizado exitosamente.");
    }

    /// <summary>
    /// Iniciar sesión con Google.
    /// </summary>
    [HttpPost("signin-google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var user = await _authService.GoogleLoginAsync(request);
            if (user == null)
            {
                return Unauthorized("No se pudo iniciar sesión con Google.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    // Elimina el GET de "google-login" para evitar duplicaciones/confusión.

    /// <summary>
    /// Solo AdminMaster puede aprobar usuarios registrados.
    /// </summary>
    [Authorize(Roles = "AdminMaster")]
    [HttpPut("approve-user/{id}")]
    public async Task<IActionResult> ApproveUser(int id)
    {
        var user = await _authService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        if (user.IsAuthorized)
        {
            return BadRequest("El usuario ya está autorizado.");
        }

        user.IsAuthorized = true; // Autorizar al usuario
        await _authService.UpdateUserAsync(user);

        return Ok("Usuario autorizado exitosamente.");
    }

    /// <summary>
    /// Cambiar el rol de un usuario.
    /// Solo AdminMaster puede realizar esta acción.
    /// </summary>
    [Authorize(Roles = "AdminMaster")]
    [HttpPut("change-role/{id}")]
    public async Task<IActionResult> ChangeUserRole(int id, [FromBody] string newRole)
    {
        var user = await _authService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        // Validar que el rol sea válido
        var validRoles = new List<string> { "AdminMaster", "Viewer", "CreatorAdmin", "EditorAdmin" };
        if (!validRoles.Contains(newRole))
        {
            return BadRequest("Rol no válido.");
        }

        user.Role = newRole; // Cambiar el rol del usuario
        await _authService.UpdateUserAsync(user);

        return Ok($"Rol del usuario cambiado a {newRole} exitosamente.");
    }
}
