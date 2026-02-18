using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Enums.Reaction;
using SocialNetwork.Write.API.Utils.Valids.Annotations.Reaction;

namespace SocialNetwork.Write.API.dto.Reaction;

public class CreateReactionDto
{
    [UniqueReactionName, MaxLength(200)]
    public required string Name { get; set; }
    
    [Required]
    public required ReactionTypeEnum Type { get; set; }
    
    [MaxLength(800)]
    public string? EmojiUrl { get; set; }
    [MaxLength(20)]
    public string? EmojiUnicode { get; set; }
    public long? DisplayOrder { get; set; }
    public bool Active { get; set; }
    public bool Visible { get; set; }
}