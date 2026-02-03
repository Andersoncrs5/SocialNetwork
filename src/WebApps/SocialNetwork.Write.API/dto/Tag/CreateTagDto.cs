using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Utils.Valids.Annotations.Tag;

namespace SocialNetwork.Write.API.dto.Tag;

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