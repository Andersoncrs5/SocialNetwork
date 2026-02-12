using SocialNetwork.Contracts.DTOs.Comment;
using SocialNetwork.Contracts.DTOs.User;

namespace SocialNetwork.Contracts.DTOs.CommentFavorite;

public class CommentFavoriteDto: BaseDto
{
    public required string CommentId { get; set; }
    public required string UserId { get; set; }

    public UserDto? User { get; set; }
    public CommentDto? Comment { get; set; }
}