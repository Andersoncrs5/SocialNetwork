using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Write.API.Modules.Comment.Dto;

public class UpdateCommentDto
{
    [MaxLength(400), MinLength(1)]
    public string? Content { get; set; }
}