using System.ComponentModel.DataAnnotations;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Comment.Validations;
using SocialNetwork.Write.API.Modules.Post.Validations;

namespace SocialNetwork.Write.API.Modules.Comment.Dto;

public class CreateCommentDto
{
    [MaxLength(400), Required, MinLength(1)]
    public required string Content { get; set; }
    [IsId, ExistsPostId] public required string PostId { get; set; }
    [ExistsCommentById] public string? ParentId { get; set; }
}