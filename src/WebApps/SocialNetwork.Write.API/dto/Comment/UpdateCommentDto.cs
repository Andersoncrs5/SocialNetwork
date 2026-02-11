using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Write.API.dto.Comment;

public class UpdateCommentDto
{
    [MaxLength(400), MinLength(1)]
    public string? Content { get; set; }
}