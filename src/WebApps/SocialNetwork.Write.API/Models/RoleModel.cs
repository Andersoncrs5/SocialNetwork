using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Write.API.Models;

public class RoleModel: IdentityRole<string>
{
    public string? Description { get; set; }
}