using SocialNetwork.Contracts.Attributes.Globals;

namespace SocialNetwork.Write.API.dto.PostTag;

public class CreatePostTagDto
{
    [IsId] public required string PostId { get; set; }
    [IsId] public required string TagId { get; set; }
}