using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserRegistryAPI.Data;
using UserRegistryAPI.Services;
using UserRegistryAPI.Repositories;
using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde el archivo .env
DotNetEnv.Env.Load(); // Asegúrate de que el archivo .env esté en la raíz del proyecto

// Configurar Kestrel para usar HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // Puerto HTTP
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // Puerto HTTPS
    });
});

// Configurar CORS para permitir todas las solicitudes (en desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configurar logging solo en modo de desarrollo
builder.Logging.AddDebug();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);  // Cambia a Debug para obtener más detalles

// Obtener la cadena de conexión desde las variables de entorno
string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
string? dbPort = Environment.GetEnvironmentVariable("DB_PORT");
string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
string? dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Verificar las credenciales de la base de datos
if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) || string.IsNullOrEmpty(dbName) ||
    string.IsNullOrEmpty(dbUsername) || string.IsNullOrEmpty(dbPassword))
{
    throw new InvalidOperationException("Las variables de entorno para la conexión a la base de datos no están configuradas.");
}

string connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUsername};Password={dbPassword}";

// Registrar DatabaseConfig como un servicio
builder.Services.AddSingleton(new DatabaseConfig(connectionString));

// Registrar DatabaseSetupService como un servicio que depende de DatabaseConfig
builder.Services.AddSingleton<DatabaseSetupService>(provider =>
{
    var databaseConfig = provider.GetRequiredService<DatabaseConfig>();
    return new DatabaseSetupService(databaseConfig);
});

// Agregar el servicio de contexto de base de datos
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString));

// Registrar el servicio JwtService
builder.Services.AddScoped<JwtService>();

// Registrar el servicio AuthService
builder.Services.AddScoped<AuthService>();

// Registrar los servicios AuthUser y IAuthUserRepository
builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();

// Registrar los servicios UserService y IUserRepository
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configuración de autenticación (consolidada para JWT y Google)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
    };
})
.AddGoogle(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? throw new ArgumentNullException("GOOGLE_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? throw new ArgumentNullException("GOOGLE_CLIENT_SECRET");
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new() { Title = "User Registry API", Version = "v1" });
    }
);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Registry API v1");
        }
    );
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<NotFoundMiddleware>(); // Este middleware presenta las rutas definidas en la API al solicitante.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ejecutar la inicialización de la base de datos y registrar el usuario AdminMaster
using (var scope = app.Services.CreateScope())
{
    var databaseSetupService = scope.ServiceProvider.GetRequiredService<DatabaseSetupService>();
    databaseSetupService.InitializeDatabase();

    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    var adminMasterEmail = Environment.GetEnvironmentVariable("ADMIN_MASTER_EMAIL");
    var adminMasterUsername = Environment.GetEnvironmentVariable("ADMIN_MASTER_USERNAME");
    var adminMasterPassword = Environment.GetEnvironmentVariable("ADMIN_MASTER_PASSWORD");

    if (string.IsNullOrEmpty(adminMasterEmail))
    {
        throw new ArgumentNullException(nameof(adminMasterEmail), "Admin master email cannot be null or empty.");
    }

    var existingAdmin = await authService.GetByEmailAsync(adminMasterEmail);

    if (existingAdmin == null)
    {
        var adminUser = new AuthUser
        {
            Username = adminMasterUsername ?? throw new ArgumentNullException(nameof(adminMasterUsername)),
            Email = adminMasterEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminMasterPassword ?? throw new ArgumentNullException(nameof(adminMasterPassword))),
            Role = "AdminMaster"
        };

        await authService.CreateAuthUserAsync(adminUser);
        Console.WriteLine("AdminMaster creado exitosamente.");
    }

    // Realizar las migraciones de la base de datos solo en el modo de desarrollo
    if (app.Environment.IsDevelopment())
    {
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();
    }
}

app.Run();
