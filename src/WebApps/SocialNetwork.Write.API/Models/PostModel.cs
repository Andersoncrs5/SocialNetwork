using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Models.Enums.Post;

namespace SocialNetwork.Write.API.Models;

public class PostModel: BaseModel
{
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Content { get; set; }
    public string? Summary { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public PostVisibilityEnum Visibility { get; set; }
    public int? ReadingTime { get; set; }
    public double RankingScore { get; set; }
    public bool IsCommentsEnabled { get; set; }
    public PostHighlightStatusEnum  HighlightStatus { get; set; }
    public ModerationStatusEnum  ModerationStatus { get; set; }
    public ReadingLevelEnum  ReadingLevel { get; set; }
    public PostTypeEnum PostType { get; set; }
    
    public required string UserId { get; set; }
    public UserModel? User { get; set; }
}