using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Contracts.DTOs.User;

namespace SocialNetwork.Contracts.DTOs.PostFavorite;

public class PostFavoriteDto: BaseDto
{
    public required string PostId { get; set; }
    public required string UserId { get; set; }
    public UserDto? User { get; set; }
    public PostDto? Post { get; set; }
}