using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Utils.Enums;

namespace SocialNetwork.Write.API.Modules.PostVote.Dto;

public record TogglePostVoteDto(
    [IsId] string postId, 
    VoteEnum vote
);