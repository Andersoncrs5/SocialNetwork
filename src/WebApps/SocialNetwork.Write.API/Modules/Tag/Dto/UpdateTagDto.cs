using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Write.API.Modules.Tag.Dto;

public class UpdateTagDto
{
    [MaxLength(150)] 
    public string? Name { get; set; }
    
    [MaxLength(250)] 
    public string? Slug { get; set; }
    [MaxLength(500)] 
    public string? Description { get; set; }
    [MaxLength(10)] 
    public string? Color { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVisible { get; set; }
    public bool? IsSystem { get; set; }
}