namespace SocialNetwork.Write.API.dto.User;

public class UpdateUserDto
{
    public string? Username { get; set; } 
    public string? FullName { get; set; } 
    public string? PasswordHash { get; set; } 
    public string? Bio { get; set; } 
    public string? CoverImageUrl { get; set; } 
    public DateTime? BirthDate { get; set; } 
    public bool? IsPrivate { get; set; } 
    public string? Language { get; set; }    
    public string? Country { get; set; }    
    public string? ImageProfileUrl { get; set; }
    
}