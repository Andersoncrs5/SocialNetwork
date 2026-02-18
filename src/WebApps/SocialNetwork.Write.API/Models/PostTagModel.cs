using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Modules.Post.Model;

namespace SocialNetwork.Write.API.Models;

public class PostTagModel: BaseModel
{
    public required string PostId { get; set; }
    public required string TagId { get; set; }
    
    public TagModel? Tag { get; set; }
    public PostModel? Post { get; set; }
}