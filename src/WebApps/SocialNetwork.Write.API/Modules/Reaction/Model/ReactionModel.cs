using SocialNetwork.Contracts.Enums.Reaction;
using SocialNetwork.Write.API.Utils.Bases;

namespace SocialNetwork.Write.API.Modules.Reaction.Model;

public class ReactionModel: BaseModel
{
    public required string Name { get; set; }
    public required ReactionTypeEnum Type { get; set; }
    public string? EmojiUrl { get; set; }
    public string? EmojiUnicode { get; set; }
    public long? DisplayOrder { get; set; }
    public bool Active { get; set; }
    public bool Visible { get; set; }
}