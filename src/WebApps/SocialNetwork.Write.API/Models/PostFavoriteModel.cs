using SocialNetwork.Write.API.Models.Bases;

namespace SocialNetwork.Write.API.Models;

public class PostFavoriteModel: BaseModel
{
    public required string PostId { get; set; }
    public required string UserId { get; set; }
    
    public UserModel? User { get; set; }
    public PostModel? Post { get; set; }
}