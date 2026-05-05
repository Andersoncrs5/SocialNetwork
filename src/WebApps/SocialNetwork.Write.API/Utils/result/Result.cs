using System.Net;

namespace SocialNetwork.Write.API.Utils.result;

public class Result
{
    public int Status { get; }
    public bool Success { get; }
    public string Message { get; }
    public IReadOnlyList<string> Errors { get; }
    public string? TraceId { get; }

    protected Result(
        int status,
        bool success,
        string message,
        IReadOnlyList<string>? errors = null,
        string? traceId = null)
    {
        Status = status;
        Success = success;
        Message = message;
        Errors = errors ?? Array.Empty<string>();
        TraceId = traceId;
    }

    public static Result Ok(
        string message = "Success",
        string? traceId = null)
        => new((int)HttpStatusCode.OK, true, message, traceId: traceId);

    public static Result Created(
        string message = "Created",
        string? traceId = null)
        => new((int)HttpStatusCode.Created, true, message, traceId: traceId);

    public static Result NoContent(
        string message = "No content",
        string? traceId = null)
        => new((int)HttpStatusCode.NoContent, true, message, traceId: traceId);

    public static Result Fail(
        string message,
        IEnumerable<string>? errors = null,
        int status = (int)HttpStatusCode.BadRequest,
        string? traceId = null)
        => new(status, false, message, errors?.ToArray(), traceId);

    public static Result NotFound(
        string message = "Resource not found",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.NotFound, traceId);

    public static Result Conflict(
        string message = "Conflict",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.Conflict, traceId);

    public static Result Unauthorized(
        string message = "Unauthorized",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.Unauthorized, traceId);

    public static Result Forbidden(
        string message = "Forbidden",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.Forbidden, traceId);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(
        int status,
        bool success,
        string message,
        T? value,
        IReadOnlyList<string>? errors = null,
        string? traceId = null)
        : base(status, success, message, errors, traceId)
    {
        Value = value;
    }

    public static Result<T> Ok(
        T value,
        string message = "Success",
        string? traceId = null)
        => new((int)HttpStatusCode.OK, true, message, value, traceId: traceId);

    public static Result<T> Created(
        T value,
        string message = "Created",
        string? traceId = null)
        => new((int)HttpStatusCode.Created, true, message, value, traceId: traceId);

    public static Result<T> Fail(
        string message,
        IEnumerable<string>? errors = null,
        int status = (int)HttpStatusCode.BadRequest,
        string? traceId = null)
        => new(status, false, message, default, errors?.ToArray(), traceId);

    public static Result<T> NotFound(
        string message = "Resource not found",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.NotFound, traceId);

    public static Result<T> Conflict(
        string message = "Conflict",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.Conflict, traceId);

    public static Result<T> Unauthorized(
        string message = "Unauthorized",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.Unauthorized, traceId);

    public static Result<T> Forbidden(
        string message = "Forbidden",
        IEnumerable<string>? errors = null,
        string? traceId = null)
        => Fail(message, errors, (int)HttpStatusCode.Forbidden, traceId);
}