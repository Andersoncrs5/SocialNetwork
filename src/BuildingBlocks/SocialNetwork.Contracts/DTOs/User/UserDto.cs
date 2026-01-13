namespace SocialNetwork.Contracts.DTOs.User;

public class UserDto: BaseDto
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
}