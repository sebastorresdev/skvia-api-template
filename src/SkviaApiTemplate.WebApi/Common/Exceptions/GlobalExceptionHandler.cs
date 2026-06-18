using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace SkviaApiTemplate.WebApi.Common.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

        if (exception is DbUpdateException dbUpdateException &&
            dbUpdateException.InnerException is PostgresException postgresException)
        {
            return await HandlePostgresExceptionAsync(httpContext, postgresException, cancellationToken);
        }

        var defaultProblemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error interno del servidor",
            Detail = "Se ha producido un error inesperado. Por favor, inténtelo de nuevo más tarde."
        };

        httpContext.Response.StatusCode = defaultProblemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(defaultProblemDetails, cancellationToken);

        return true;
    }

    private static async Task<bool> HandlePostgresExceptionAsync(HttpContext httpContext, PostgresException postgresException, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        switch (postgresException.SqlState)
        {
            // Código de violación de constraint único
            case "23505":
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflicto de datos",
                    Detail = "Se ha producido un conflicto de datos debido a una operación concurrente. Por favor, inténtelo de nuevo."
                };
                break;

            // Código de violación de constraint de clave foránea
            case "23503":
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Operación no permitida",
                    Detail = "No se puede eliminar el registro porque tiene datos históricos asociados. Considere inactivar el registro en su lugar."
                };
                break;

            default:
                // No manejamos esta excepción de Postgres, dejamos que siga el flujo al error 500
                var defaultProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Error interno de base de datos",
                    Detail = "Se ha producido un error inesperado en la base de datos."
                };
                httpContext.Response.StatusCode = defaultProblemDetails.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(defaultProblemDetails, cancellationToken);
                return true;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}