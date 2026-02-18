using SocialNetwork.Contracts.Enums.Reaction;

namespace SocialNetwork.Write.API.Modules.Reaction.Dto;

public class UpdateReactionDto
{
    public string? Name { get; set; }
    public ReactionTypeEnum? Type { get; set; }
    public string? EmojiUrl { get; set; }
    public string? EmojiUnicode { get; set; }
    public long? DisplayOrder { get; set; }
    public bool? Active { get; set; }
    public bool? Visible { get; set; }
}