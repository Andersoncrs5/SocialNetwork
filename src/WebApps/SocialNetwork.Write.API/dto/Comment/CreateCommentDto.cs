using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Utils.Valids.Annotations.Comment;

namespace SocialNetwork.Write.API.dto.Comment;

public class CreateCommentDto
{
    [MaxLength(400), Required, MinLength(1)]
    public required string Content { get; set; }
    [IsId] public required string PostId { get; set; }
    [ExistsCommentById] public string? ParentId { get; set; }
}