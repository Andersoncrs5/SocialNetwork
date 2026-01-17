using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Utils.Valids.Annotations.User;

namespace SocialNetwork.Write.API.dto.User;

public class CreateUserDto
{
    [UniqueUsername]
    [StringLength(100, ErrorMessage = "Max size of 100")]
    public required string Username { get; set; } 
    
    [StringLength(200, ErrorMessage = "Max size of 200")]
    public string? FullName { get; set; }
    
    [UniqueEmail]
    [EmailConstraint]
    public required string Email { get; set; } 

    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Max size of 50", MinimumLength = 6)] 
    public required string PasswordHash { get; set; }
}