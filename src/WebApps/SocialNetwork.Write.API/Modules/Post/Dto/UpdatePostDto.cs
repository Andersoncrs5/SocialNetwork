using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Contracts.Utils.Enums;

namespace SocialNetwork.Write.API.Modules.Post.Dto;

public class UpdatePostDto
{
    [MaxLength(150), MinLength(5)]
    public string? Title { get; set; }
    [MaxLength(250), MinLength(5)]
    public string? Slug { get; set; }
    
    [MaxLength(700), MinLength(100)]
    public string? Content { get; set; }
    
    [MaxLength(300)]
    public string? Summary { get; set; }
    
    [MaxLength(800)]
    public string? FeaturedImageUrl { get; set; }
    public PostVisibilityEnum? Visibility { get; set; }
    public int? ReadingTime { get; set; }
    public bool? IsCommentsEnabled { get; set; }
    public bool? Pinned { get; set; }
    public ReadingLevelEnum?  ReadingLevel { get; set; }
    public LanguageEnum? Language { get; set; } 
    public PostTypeEnum? PostType { get; set; }
}