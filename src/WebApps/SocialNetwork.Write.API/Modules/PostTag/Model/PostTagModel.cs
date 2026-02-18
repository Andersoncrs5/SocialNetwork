using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Tag.Model;

namespace SocialNetwork.Write.API.Modules.PostTag.Model;

public class PostTagModel: BaseModel
{
    public required string PostId { get; set; }
    public required string TagId { get; set; }
    
    public TagModel? Tag { get; set; }
    public PostModel? Post { get; set; }
}