using UserRegistryAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using UserRegistryAPI.Services;
using UserRegistryAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

// Obtener la cadena de conexión desde appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Registrar la cadena de conexión
builder.Services.AddSingleton(connectionString);

// Registrar DatabaseConfig como un servicio
builder.Services.AddSingleton<DatabaseConfig>();

// Registrar DatabaseSetupService como un servicio
builder.Services.AddSingleton<DatabaseSetupService>();

// Agregar el servicio de contexto de base de datos
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar los servicios UserService y IUserRepository
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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

app.UseMiddleware<NotFoundMiddleware>(); //Este middleware presenta las rutas definidas en la API al Solicitante.


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ejecutar la inicialización de la base de datos aquí
using (var scope = app.Services.CreateScope())
{
    var databaseSetupService = scope.ServiceProvider.GetRequiredService<DatabaseSetupService>();
    databaseSetupService.InitializeDatabase();

    // Agregar países desde archivo CSV utilizando DataLoaders/CountryLoader.cs Solo para pruebas
    //var databaseConfig = scope.ServiceProvider.GetRequiredService<DatabaseConfig>();
    //var countryLoader = new CountryLoader(databaseConfig);
    //await countryLoader.LoadCountriesAsync("Data/pais.csv");

    // Realizar las migraciones de la base de datos solo en el modo de desarrollo
    if (app.Environment.IsDevelopment())
    {
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.Database.Migrate();
    }
}

app.Run();
