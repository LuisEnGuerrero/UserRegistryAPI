using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config), "La configuración es requerida.");
    }

    /// <summary>
    /// Genera un token JWT para el usuario autenticado.
    /// </summary>
    /// <param name="user">El usuario autenticado para el cual se genera el token.</param>
    /// <returns>Un string con el token JWT generado.</returns>
    public string GenerateJwtToken(AuthUser user)
    {
        // Verificar si la clave JWT está configurada correctamente
        var key = _config["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_config), "Jwt:Key is null.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Definir los claims del token, incluyendo el rol del usuario
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)  // Incluye el rol del usuario en los claims
        };

        // Crear el token con los claims y la expiración de 30 minutos
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);  // Devolver el token como string
    }
}
