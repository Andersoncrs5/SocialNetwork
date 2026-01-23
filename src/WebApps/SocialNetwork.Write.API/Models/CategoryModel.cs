using SocialNetwork.Write.API.Models.Bases;

namespace SocialNetwork.Write.API.Models;

public class CategoryModel: BaseModel
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
    public CategoryModel? Parent { get; set; }
    public ICollection<CategoryModel> Children { get; set; } = new List<CategoryModel>();
}