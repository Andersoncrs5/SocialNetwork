using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Models.Enums.Post;

namespace SocialNetwork.Write.API.Models;

public class CommentModel: BaseModel
{
    public required string Content { get; set; }
    public required string PostId { get; set; }
    public PostModel? Post { get; set; }
    
    public required string UserId { get; set; }
    public UserModel? User { get; set; }

    public string? ParentId { get; set; }
    public CommentModel? Parent { get; set; }
    
    public double SentimentScore { get; set; }
    public bool IsEdited { get; set; }
    public ModerationStatusEnum ModerationStatus { get; set; }
    public ICollection<CommentModel> Replies { get; set; } = new List<CommentModel>();
}