using SocialNetwork.Contracts.DTOs.Tag;

namespace SocialNetwork.Contracts.DTOs.PostTag;

public class PostTagDto: BaseDto
{
    public required string PostId { get; set; }
    public required string TagId { get; set; }
    
    public TagDto? Tag { get; set; }
}