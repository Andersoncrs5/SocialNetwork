namespace SocialNetwork.Contracts.configs.jwt;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public double TokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInMinutes { get; set; }
}