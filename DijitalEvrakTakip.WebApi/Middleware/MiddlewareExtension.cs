namespace DijitalEvrakTakip.WebApi.Middleware;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseMiddlewareExtensions(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorMiddleware>();
        return app;
    }
}
