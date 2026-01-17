using SocialNetwork.Contracts.Attributes.Globals;

namespace SocialNetwork.Write.API.dto.User;

public class LoginUserDto
{
    [EmailConstraint]
    public required string Email { get; set; }
    
    public required string Password { get; set; }
}