using SocialNetwork.Contracts.DTOs.User;

namespace SocialNetwork.Contracts.DTOs.PostCategory;

public class PostCategoryDto: BaseDto
{
    public required string PostId { get; set; }
    public required string CategoryId { get; set; }
    public uint Order { get; set; }
}