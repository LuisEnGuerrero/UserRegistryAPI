using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Threading.Tasks;

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;

    public NotFoundMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, LinkGenerator linkGenerator, EndpointDataSource endpointDataSource)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            context.Response.ContentType = "application/json";

            var routes = endpointDataSource.Endpoints
                .OfType<RouteEndpoint>()
                .Select(endpoint => new
                {
                    Route = endpoint.RoutePattern.RawText,
                    Method = string.Join(", ", endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods ?? new[] { "GET" })
                })
                .ToList();

            var response = new
            {
                Message = "La ruta solicitada no existe. Aquí están las rutas disponibles:",
                AvailableRoutes = routes
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
