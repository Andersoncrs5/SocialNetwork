using System;
using SocialNetwork.Contracts.DTOs.User;

namespace SocialNetwork.Contracts.Utils.Res.http;

public class ResponseTokens
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? ExpiredAtRefreshToken { get; set; }
    public UserDto? User { get; set; }
    public IList<string>? Roles { get; set; }
}