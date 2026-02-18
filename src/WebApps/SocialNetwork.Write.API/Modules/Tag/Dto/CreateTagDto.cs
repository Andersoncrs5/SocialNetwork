using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Tag.Validations;

namespace SocialNetwork.Write.API.Modules.Tag.Dto;

public class CreateTagDto
{
    [MaxLength(150), UniqueTagName] 
    public required string Name { get; set; }
    [MaxLength(250), SlugConstraint, UniqueTagSlug] 
    public required string Slug { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(10)] public string? Color { get; set; }
    [Required] public bool IsActive { get; set; }
    [Required] public bool IsVisible { get; set; }
    [Required] public bool IsSystem { get; set; }
}