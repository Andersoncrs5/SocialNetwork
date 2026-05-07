using SocialNetwork.Contracts.Attributes.Globals;

namespace SocialNetwork.Write.API.Modules.PostReactions.Dto;

public record TogglePostReactionDto(
    [IsId] string PostId,
    [IsId] string ReactionId
);