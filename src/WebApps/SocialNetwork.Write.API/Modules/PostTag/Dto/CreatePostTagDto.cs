using SocialNetwork.Contracts.Attributes.Globals;

namespace SocialNetwork.Write.API.Modules.PostTag.Dto;

public class CreatePostTagDto
{
    [IsId] public required string PostId { get; set; }
    [IsId] public required string TagId { get; set; }
}