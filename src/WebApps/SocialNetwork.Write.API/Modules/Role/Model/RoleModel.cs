using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Write.API.Modules.Role.Model;

public class RoleModel: IdentityRole<string>
{
    public string? Description { get; set; }
}