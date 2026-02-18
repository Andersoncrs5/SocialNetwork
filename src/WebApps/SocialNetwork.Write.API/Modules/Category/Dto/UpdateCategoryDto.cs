using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Write.API.Modules.Category.Dto;

public class UpdateCategoryDto
{
    [MaxLength(150)]
    public string? Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(800)]
    public string? IconName { get; set; }
    
    [MaxLength(7)]
    public string? Color { get; set; }
    
    [MaxLength(250)]
    public string? Slug { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVisible { get; set; }
    public uint? DisplayOrder { get; set; }
    
    public string? ParentId { get; set; } 
    
    public bool? BecameRoot { get; set; }
}