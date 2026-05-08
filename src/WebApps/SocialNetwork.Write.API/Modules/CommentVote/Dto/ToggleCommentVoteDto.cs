using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Utils.Enums;

namespace SocialNetwork.Write.API.Modules.CommentVote.Dto;

public record ToggleCommentVoteDto(
    [IsId] string commentId, 
    VoteEnum vote
);