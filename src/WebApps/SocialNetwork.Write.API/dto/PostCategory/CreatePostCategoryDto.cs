using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Utils.Valids.Annotations.Category;
using SocialNetwork.Write.API.Utils.Valids.Annotations.Post;

namespace SocialNetwork.Write.API.dto.PostCategory;

public class CreatePostCategoryDto
{
    [IsId] public required string PostId { get; set; }
    [IsId] public required string CategoryId { get; set; }
    public uint Order { get; set; }
}