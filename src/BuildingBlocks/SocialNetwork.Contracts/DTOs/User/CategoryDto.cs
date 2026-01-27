namespace SocialNetwork.Contracts.DTOs.User;

public class CategoryDto: BaseDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public string? Color { get; set; }
    public required string Slug { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    public uint? DisplayOrder { get; set; }
    public string? ParentId { get; set; } 
}