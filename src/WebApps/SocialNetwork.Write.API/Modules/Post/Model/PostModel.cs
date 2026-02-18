using SocialNetwork.Contracts.Enums.Post;
using SocialNetwork.Contracts.Utils.Enums;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Models.Enums.Post;

namespace SocialNetwork.Write.API.Modules.Post.Model;

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
    public decimal EstimatedValue { get; set; } = 0;
    public bool IsCommentsEnabled { get; set; }
    public bool Pinned { get; set; }
    public PostHighlightStatusEnum  HighlightStatus { get; set; }
    public ModerationStatusEnum  ModerationStatus { get; set; }
    public ReadingLevelEnum  ReadingLevel { get; set; }
    public PostTypeEnum PostType { get; set; }
    public LanguageEnum Language { get; set; } = LanguageEnum.Undefined;
    public string? ParentId { get; set; } 
    
    public required string UserId { get; set; }
    public UserModel? User { get; set; }
    public PostModel? Parent { get; set; }
    
    public ICollection<PostCategoryModel> PostCategories { get; set; } = new List<PostCategoryModel>();
    public ICollection<PostModel> Children { get; set; } = new List<PostModel>();
    
    public ICollection<PostTagModel> PostTags { get; set; } = new List<PostTagModel>();
    public ICollection<PostFavoriteModel> Favorites { get; set; } = new List<PostFavoriteModel>();
    
}