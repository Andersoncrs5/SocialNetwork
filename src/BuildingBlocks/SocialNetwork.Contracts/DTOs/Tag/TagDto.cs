namespace SocialNetwork.Contracts.DTOs.Tag;

public class TagDto: BaseDto
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    public bool IsSystem { get; set; }
}