using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils.Bases;

namespace SocialNetwork.Write.API.Modules.PostFavorite.Model;

public class PostFavoriteModel: BaseModel
{
    public required string PostId { get; set; }
    public required string UserId { get; set; }
    
    public UserModel? User { get; set; }
    public PostModel? Post { get; set; }
}