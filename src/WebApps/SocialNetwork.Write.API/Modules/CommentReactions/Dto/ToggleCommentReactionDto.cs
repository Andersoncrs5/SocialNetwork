using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;

namespace SocialNetwork.Write.API.Modules.CommentReactions.Dto;

public class ToggleCommentReactionDto
{
    [IsId] public required string ReactionId { get; set; }
    [IsId] public required string CommentId { get; set; }
}