/// <summary>
/// Representa a un usuario autenticado en el sistema con diferentes roles.
/// </summary>
public class AuthUser
{
    /// <summary>
    /// Identificador único del usuario en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de usuario (único).
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Hash de la contraseña del usuario.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Rol del usuario (AdminMaster, Viewer, CreatorAdmin, EditorAdmin).
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Indica si el usuario utiliza autenticación de Google.
    /// </summary>
    public bool IsGoogleAuth { get; set; } = false;

    /// <summary>
    /// ID de Google del usuario (se usa solo si IsGoogleAuth es true).
    /// </summary>
    public string? GoogleId { get; set; }

    /// <summary>
    /// Indica si el usuario está autorizado para hacer login.
    /// </summary>
    public bool IsAuthorized { get; set; } = false;

    // Constructor predeterminado
    public AuthUser() { }

    // Constructor que permite crear usuarios con las propiedades esenciales
    public AuthUser(string username, string email, string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("El nombre de usuario no puede estar vacío.");
        
        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            throw new ArgumentException("El correo electrónico no es válido.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de la contraseña no puede estar vacío.");

        if (!IsValidRole(role))
            throw new ArgumentException("El rol proporcionado no es válido.");

        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    // Método para validar si el correo electrónico tiene un formato válido
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Método para validar que el rol sea uno de los permitidos
    private bool IsValidRole(string role)
    {
        var validRoles = new List<string> { "AdminMaster", "Viewer", "CreatorAdmin", "EditorAdmin" };
        return validRoles.Contains(role);
    }
}
