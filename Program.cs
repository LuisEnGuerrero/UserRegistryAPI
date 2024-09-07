using UserRegistryAPI.Data;
using Microsoft.Extensions.DependencyInjection;

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

// Obtener la cadena de conexión desde appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Registrar la cadena de conexión
builder.Services.AddSingleton(connectionString);

// Registrar DatabaseConfig como un servicio
builder.Services.AddSingleton<DatabaseConfig>();

// Registrar DatabaseSetupService como un servicio
builder.Services.AddSingleton<DatabaseSetupService>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ejecutar la inicialización de la base de datos aquí
using (var scope = app.Services.CreateScope())
{
    var databaseSetupService = scope.ServiceProvider.GetRequiredService<DatabaseSetupService>();
    databaseSetupService.InitializeDatabase();

    // Agregar países desde archivo CSV utilizando DataLoaders CountryLoader
    var databaseConfig = scope.ServiceProvider.GetRequiredService<DatabaseConfig>();
    var countryLoader = new CountryLoader(databaseConfig);
    await countryLoader.LoadCountriesAsync("Data/countries.csv");
}

app.Run();