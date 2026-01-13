using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Write.API.Models;

public class RoleModel: IdentityRole
{
    public string? Description { get; set; }
}