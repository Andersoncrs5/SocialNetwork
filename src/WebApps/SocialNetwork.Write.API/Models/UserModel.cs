using System;
using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Write.API.Models;

public class UserModel: IdentityUser
{
    public string? FullName { get; set; }
    
    public string? Bio { get; set; } 
    public string? CoverImageUrl { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsPrivate { get; set; }  
    public string? Language { get; set; }    
    public string? Country { get; set; }    
    
    public string? ImageProfileUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}