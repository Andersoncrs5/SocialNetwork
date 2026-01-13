using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Utils.Res.http;

namespace SocialNetwork.Write.API.Configs.Exception;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        System.Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        // Mapeamento de Exception -> Status Code
        var (statusCode, message) = exception switch
        {
            // Equivalente ao ModelNotFoundException
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            
            // Equivalente ao OptimisticLockingFailureException
            DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "This record was updated by another user. Please refresh."),
            
            // Equivalente ao DataIntegrityViolationException
            DbUpdateException => (HttpStatusCode.Conflict, "Database integrity violation."),

            // Erros de Validação (como o seu [IsId] ou [Required])
            BadHttpRequestException => (HttpStatusCode.BadRequest, "Validation failed."),

            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        var response = new ResponseHttp<object>(
            null,
            message,
            Guid.NewGuid().ToString(),
            0,
            false,
            DateTime.UtcNow
        );

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; 
    }
}