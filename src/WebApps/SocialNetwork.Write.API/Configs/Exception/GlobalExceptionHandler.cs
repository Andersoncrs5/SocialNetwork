using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Contracts.Utils.Res.http;
using SocialNetwork.Write.API.Configs.Exception.classes;

namespace SocialNetwork.Write.API.Configs.Exception;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        System.Exception exception, 
        CancellationToken cancellationToken
        )
    {
        var (statusCode, message, isBusinessError) = exception switch
        {
            ModelNotFoundException => (HttpStatusCode.NotFound, exception.Message, true),
            
            DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Este registro foi atualizado por outro usuário.", false),
            
            DbUpdateException => (HttpStatusCode.Conflict, "Violação de integridade no banco de dados.", false),

            BadHttpRequestException => (HttpStatusCode.BadRequest, "Falha na validação da requisição.", true),
            
            ConflictValueException => (HttpStatusCode.BadRequest, exception.Message, true),
            
            UnauthenticatedException => (HttpStatusCode.Unauthorized, exception.Message, true),

            _ => (HttpStatusCode.InternalServerError, $"Ocorreu um erro inesperado no servidor." , false)
        };

        // 2. Lógica de Log Inteligente para Observabilidade
        if (isBusinessError)
        {
            // LogWarning não costuma disparar gatilhos de PagerDuty/Grafana
            logger.LogWarning("Business Violation: {Message} | TraceId: {TraceId}", 
                message, httpContext.TraceIdentifier);
        }
        else
        {
            // LogError dispara alertas de infraestrutura
            logger.LogError(exception, "Critical Error: {Message} | TraceId: {TraceId}", 
                exception.Message, httpContext.TraceIdentifier);
        }

        // 3. Resposta padronizada
        var response = new ResponseHttp<object>(
            Data: null,
            Message: message,
            TraceId: httpContext.TraceIdentifier,
            Success: false,
            Timestamp: DateTime.UtcNow,
            DetailsError: exception.Message
        );

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; 
    }
}