using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Threading.Tasks;

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor del middleware. Recibe el siguiente middleware en la cadena de ejecución.
    /// </summary>
    /// <param name="next">Delegate para la siguiente fase del pipeline</param>
    public NotFoundMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Método principal que intercepta las solicitudes HTTP y maneja los casos de 404 (ruta no encontrada).
    /// </summary>
    /// <param name="context">El contexto HTTP actual</param>
    /// <param name="linkGenerator">Generador de enlaces para crear rutas disponibles</param>
    /// <param name="endpointDataSource">Fuente de datos para obtener los endpoints disponibles</param>
    /// <returns>Tarea asincrónica</returns>
    public async Task InvokeAsync(HttpContext context, LinkGenerator linkGenerator, EndpointDataSource endpointDataSource)
    {
        // Continuar con la siguiente fase del pipeline de ejecución
        await _next(context);

        // Verificar si la respuesta tiene un código de estado 404 (No Encontrado)
        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            // Cambiar el tipo de contenido de la respuesta a JSON
            context.Response.ContentType = "application/json";

            // Obtener todas las rutas disponibles desde el EndpointDataSource
            var routes = endpointDataSource.Endpoints
                .OfType<RouteEndpoint>()
                .Select(endpoint => new
                {
                    Route = endpoint.RoutePattern.RawText, // Obtener el patrón de ruta
                    Method = string.Join(", ", endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods ?? new[] { "GET" }) // Obtener los métodos HTTP permitidos
                })
                .ToList();

            // Crear el objeto de respuesta con las rutas disponibles
            var response = new
            {
                Message = "La ruta solicitada no existe. Aquí están las rutas disponibles:",
                AvailableRoutes = routes
            };

            // Escribir la respuesta en formato JSON
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
