using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Contracts.Utils.Enums;
using SocialNetwork.Write.API.Modules.Post.Validations;

namespace SocialNetwork.Write.API.Modules.Post.Dto;

public class CreatePostDto
{
    [MaxLength(150), MinLength(5), Required]
    public required string Title { get; set; }
    
    [SlugConstraint, MaxLength(250), MinLength(5), UniquePostSlug]
    public required string Slug { get; set; }
    
    [Required ,MaxLength(700), MinLength(100)]
    public required string Content { get; set; }
    
    [MaxLength(300)]
    public string? Summary { get; set; }
    
    [MaxLength(800)]
    public string? FeaturedImageUrl { get; set; }
    
    [Required]
    public PostVisibilityEnum Visibility { get; set; }
    
    public int? ReadingTime { get; set; }
    
    [Required]
    public bool IsCommentsEnabled { get; set; }
    
    [Required]
    public LanguageEnum Language { get; set; } = LanguageEnum.Undefined;
    
    [Required]
    public ReadingLevelEnum  ReadingLevel { get; set; }
    
    [Required]
    public PostTypeEnum PostType { get; set; }
    
    public string? ParentId { get; set; }
}