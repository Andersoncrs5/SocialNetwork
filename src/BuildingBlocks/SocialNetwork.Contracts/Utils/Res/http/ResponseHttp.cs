using System;

namespace SocialNetwork.Contracts.Utils.Res.http;

public record ResponseHttp<T>(
    T? Data,
    string Message,
    string TraceId,
    bool Success,
    DateTime Timestamp,
    string? DetailsError = null,
    string? Path = null,
    UInt16 ApiVersion = 1
);