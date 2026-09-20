using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Persistance.Context;
using FluentValidation;

namespace DijitalEvrakTakip.WebApi.Middleware;

public sealed class ErrorMiddleware : IMiddleware
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ErrorMiddleware(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            try
            {
                await LogExceptionToDatabaseAsync(ex, context.Request);
            }
            catch
            {
                // Logging the original exception must never hide it behind a secondary failure
                // (e.g. the same DB outage that caused ex in the first place).
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        if (ex.GetType() == typeof(ValidationException))
        {
            return context.Response.WriteAsync(new ValidationErrorDetails
            {
                Errors = ((ValidationException)ex).Errors.Select(x =>
                x.PropertyName),
                StatusCode = 403
            }.ToString());
        }

        return context.Response.WriteAsync(new ErrorResult
        {
            Message = ex.Message,
            StatusCode = 500
        }.ToString());
    }

    private async Task LogExceptionToDatabaseAsync(Exception ex, HttpRequest request)
    {
        ErrorLog errorLog = new()
        {
            ErrorMessage = ex.ToString(),
            StackTrace = ex.StackTrace,
            RequestPath = request.Path,
            RequestMethod = request.Method,
            TimeStamp = DateTime.Now
        };

        // Use a fresh scope/DbContext instead of the request's own scoped context:
        // if ex originated from that same context (e.g. a failed SaveChanges), it can be
        // left in a state where reusing it to log the error throws a second exception.
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Set<ErrorLog>().AddAsync(errorLog, default);
        await context.SaveChangesAsync(default);
    }
}
