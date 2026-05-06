using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialNetwork.Contracts.Utils.Exceptions;
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
            
            InternalServerErrorException => (HttpStatusCode.Conflict, "Error internal in server", false),

            BadHttpRequestException => (HttpStatusCode.BadRequest, "Falha na validação da requisição.", true),
            
            ConflictValueException => (HttpStatusCode.BadRequest, exception.Message, true),
            
            UnauthenticatedException => (HttpStatusCode.Unauthorized, exception.Message, true),
            
            SelfReferencingHierarchyException => (HttpStatusCode.UnprocessableEntity, exception.Message, true),
            
            IdentityOperationException => (HttpStatusCode.UnprocessableEntity, exception.Message, true),
            
            ForbiddenException => (HttpStatusCode.Forbidden, exception.Message, true),
            
            ResourceOwnerMismatchException => (HttpStatusCode.Forbidden, exception.Message, true),

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

/*

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
   {
       public async ValueTask<bool> TryHandleAsync(
           HttpContext httpContext,
           Exception exception,
           CancellationToken cancellationToken)
       {
           var (statusCode, message, isBusinessError) = exception switch
           {
               ModelNotFoundException => (HttpStatusCode.NotFound, exception.Message, true),
   
               DbUpdateConcurrencyException => (
                   HttpStatusCode.Conflict,
                   "Este registro foi atualizado por outro usuário.",
                   false
               ),
   
               DbUpdateException dbEx when dbEx.IsForeignKeyViolation() => HandleForeignKey(dbEx),
   
               DbUpdateException dbEx when dbEx.IsDuplicateEntry() => (
                   HttpStatusCode.Conflict,
                   "Já existe um registro com esses dados.",
                   true
               ),
   
               DbUpdateException dbEx when dbEx.IsDataTooLong() => (
                   HttpStatusCode.BadRequest,
                   "Um dos campos excede o tamanho permitido.",
                   true
               ),
   
               InternalServerErrorException => (
                   HttpStatusCode.InternalServerError,
                   "Erro interno no servidor.",
                   false
               ),
   
               BadHttpRequestException => (
                   HttpStatusCode.BadRequest,
                   "Falha na validação da requisição.",
                   true
               ),
   
               ConflictValueException => (
                   HttpStatusCode.BadRequest,
                   exception.Message,
                   true
               ),
   
               UnauthenticatedException => (
                   HttpStatusCode.Unauthorized,
                   exception.Message,
                   true
               ),
   
               SelfReferencingHierarchyException => (
                   HttpStatusCode.UnprocessableEntity,
                   exception.Message,
                   true
               ),
   
               IdentityOperationException => (
                   HttpStatusCode.UnprocessableEntity,
                   exception.Message,
                   true
               ),
   
               ForbiddenException => (
                   HttpStatusCode.Forbidden,
                   exception.Message,
                   true
               ),
   
               ResourceOwnerMismatchException => (
                   HttpStatusCode.Forbidden,
                   exception.Message,
                   true
               ),
   
               _ => (
                   HttpStatusCode.InternalServerError,
                   "Ocorreu um erro inesperado no servidor.",
                   false
               )
           };
   
           if (isBusinessError)
           {
               logger.LogWarning(
                   "Business Violation: {Message} | TraceId: {TraceId}",
                   message,
                   httpContext.TraceIdentifier
               );
           }
           else
           {
               logger.LogError(
                   exception,
                   "Critical Error: {Message} | TraceId: {TraceId}",
                   exception.Message,
                   httpContext.TraceIdentifier
               );
           }
   
           var response = new ResponseHttp<object>(
               Data: null,
               Message: message,
               TraceId: httpContext.TraceIdentifier,
               Success: false,
               Timestamp: DateTime.UtcNow,
               DetailsError: exception.Message
           );
   
           httpContext.Response.StatusCode = (int)statusCode;
           httpContext.Response.ContentType = "application/json";
   
           await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
           return true;
       }
   
       private static (HttpStatusCode StatusCode, string Message, bool IsBusinessError) HandleForeignKey(DbUpdateException ex)
       {
           var current = ex as Exception;
           while (current?.InnerException != null)
               current = current.InnerException;
   
           var message = current?.Message ?? ex.Message;
   
           if (message.Contains("FK_PostVotes_Posts_PostId", StringComparison.OrdinalIgnoreCase))
               return (HttpStatusCode.NotFound, "Post not found.", true);
   
           if (message.Contains("FK_PostVotes_Users_UserId", StringComparison.OrdinalIgnoreCase))
               return (HttpStatusCode.NotFound, "User not found.", true);
   
           return (HttpStatusCode.NotFound, "User or post not found.", true);
       }
   }
   
*/