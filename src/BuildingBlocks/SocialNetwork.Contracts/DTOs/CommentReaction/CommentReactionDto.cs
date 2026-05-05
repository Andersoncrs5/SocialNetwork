using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Contracts.DTOs.Reaction;
using SocialNetwork.Contracts.DTOs.User;

namespace SocialNetwork.Contracts.DTOs.CommentReaction;

public class CommentReactionDto: BaseDto
{
    [IsId] public required string UserId { get; set; }
    [IsId] public required string ReactionId { get; set; } 
    [IsId] public required string CommentId { get; set; }

    public UserDto? User { get; set; }
    public ReactionDto? Reaction { get; set; }
    public CommentDto? Comment { get; set; }
}