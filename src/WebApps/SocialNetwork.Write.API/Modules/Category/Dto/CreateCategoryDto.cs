using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Category.Validations;

namespace SocialNetwork.Write.API.Modules.Category.Dto;

public class CreateCategoryDto
{
    [UniqueCategoryName, MaxLength(150), MinLength(2)]
    public required string Name { get; set; }
    
    [UniqueCategorySlug, SlugConstraint, MaxLength(250), MinLength(1)]
    public required string Slug { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(800)]
    public string? IconName { get; set; }
    
    [MaxLength(6)]
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    
    public uint? DisplayOrder { get; set; }
    
    [ExistsCategoryById]
    public string? ParentId { get; set; } 
}