using System.Text.Json;
using BookReviews.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api_libreria.Middleware;

/// <summary>
/// Manejo centralizado de excepciones (#10). Traduce las excepciones de la capa
/// de aplicación a códigos HTTP y devuelve un ProblemDetails uniforme.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Acceso denegado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto"),
            ValidationException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "No autorizado"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Error no controlado en la solicitud {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogInformation("Solicitud rechazada ({Status}): {Message}", status, exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            // No exponemos detalles internos en errores 500.
            Detail = status == StatusCodes.Status500InternalServerError
                ? "Ocurrió un error inesperado."
                : exception.Message
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
