using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Write.API.Modules.User.Dto;

public class UpdateUserDto
{
    [StringLength(100, ErrorMessage = "Max size of 100")]
    public string? Username { get; set; } 
    
    [StringLength(200, ErrorMessage = "Max size of 200")]
    public string? FullName { get; set; } 
    
    [StringLength(50, ErrorMessage = "Max size of 50")] 
    public string? PasswordHash { get; set; } 
    
    [StringLength(800, ErrorMessage = "Bio Max size of 800")] 
    public string? Bio { get; set; } 
    
    [StringLength(800, ErrorMessage = "CoverImageUrl Max size of 800")] 
    public string? CoverImageUrl { get; set; } 
    public DateTime? BirthDate { get; set; } 
    public bool? IsPrivate { get; set; } 
    
    [StringLength(4, ErrorMessage = "Max size of 4")] 
    public string? Language { get; set; }    
    
    [StringLength(100, ErrorMessage = "Max size of 100")] 
    public string? Country { get; set; }    
    
    [StringLength(800, ErrorMessage = "Max size of 800")] 
    public string? ImageProfileUrl { get; set; }
    
}