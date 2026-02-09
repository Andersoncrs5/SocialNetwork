using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Utils.Enums;
using SocialNetwork.Write.API.Models.Enums.Post;

namespace SocialNetwork.Write.API.dto.Posts;

public class PostDto: BaseDto
{
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Content { get; set; }
    public string? Summary { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public PostVisibilityEnum Visibility { get; set; }
    public int? ReadingTime { get; set; }
    public bool IsCommentsEnabled { get; set; }
    public bool Pinned { get; set; }
    public string? ParentId { get; set; }
    public LanguageEnum Language { get; set; } 
    public decimal EstimatedValue { get; set; } = 0;
    
    public ReadingLevelEnum  ReadingLevel { get; set; }
    public PostTypeEnum PostType { get; set; }
    public required string UserId { get; set; }
}