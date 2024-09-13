/// <summary>
/// Modelo para la solicitud de login mediante Google.
/// El token es proporcionado por el servicio de autenticación de Google.
/// </summary>
public class GoogleLoginRequest
{
    /// <summary>
    /// Token proporcionado por Google para la autenticación.
    /// </summary>
    public string Token { get; set; }

    // Constructor que valida que el token no sea nulo ni vacío
    public GoogleLoginRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("El token de Google no puede estar vacío.");
        }

        Token = token;
    }
}
