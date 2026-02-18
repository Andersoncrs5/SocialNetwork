using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils.Bases;

namespace SocialNetwork.Write.API.Modules.CommentFavorite.Model;

public class CommentFavoriteModel: BaseModel
{
    public required string CommentId { get; set; }
    public required string UserId { get; set; }
    
    public CommentModel? Comment { get; set; }
    public UserModel? User { get; set; }
}