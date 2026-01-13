namespace SocialNetwork.Contracts.Utils.Res.http;

public record ResponseHttp<T>(
    T? Data,
    string Message,
    string TraceId,
    int ErrorCode,
    bool Success,
    DateTime Timestamp
);