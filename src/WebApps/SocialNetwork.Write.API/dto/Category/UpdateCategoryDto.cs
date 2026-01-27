using SocialNetwork.Write.API.Utils.Valids.Annotations.Category;

namespace SocialNetwork.Write.API.dto.Category;

public class UpdateCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public string? Color { get; set; }
    public string? Slug { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVisible { get; set; }
    public uint? DisplayOrder { get; set; }
    public string? ParentId { get; set; } 
}