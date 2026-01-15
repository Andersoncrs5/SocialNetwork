namespace SocialNetwork.Contracts.Utils.Res.http;

public class ResponseTokens
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? ExpiredAtRefreshToken { get; set; }
}