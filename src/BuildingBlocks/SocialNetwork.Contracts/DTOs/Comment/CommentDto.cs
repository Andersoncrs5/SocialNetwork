using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.DTOs.User;
using SocialNetwork.Write.API.Models.Enums.Post;

namespace SocialNetwork.Contracts.DTOs.Comment;

public class CommentDto: BaseDto
{
    public required string Content { get; set; }
    public double SentimentScore { get; set; }
    public bool IsEdited { get; set; }
    public ModerationStatusEnum ModerationStatus { get; set; }
     
    public required string PostId { get; set; }
    
    public required string UserId { get; set; }
    public UserDto? User { get; set; }
    
    public string? ParentId { get; set; }
    
    public PostDto? Post { get; set; }
}