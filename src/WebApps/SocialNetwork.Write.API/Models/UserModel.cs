using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Write.API.Models;

public class UserModel: IdentityUser
{
    public string? FullName { get; set; }
    public string? ImageProfileUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}