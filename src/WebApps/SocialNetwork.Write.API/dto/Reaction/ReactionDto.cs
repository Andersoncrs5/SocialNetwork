using SocialNetwork.Contracts.DTOs;
using SocialNetwork.Contracts.Enums.Reaction;

namespace SocialNetwork.Write.API.dto.Reaction;

public class ReactionDto: BaseDto
{
    public required string Name { get; set; }
    public required ReactionTypeEnum Type { get; set; }
    public string? EmojiUrl { get; set; }
    public string? EmojiUnicode { get; set; }
    public long? DisplayOrder { get; set; }
    public bool Active { get; set; }
    public bool Visible { get; set; }
}