using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Modules.Comment.Model;

namespace SocialNetwork.Write.API.Models;

public class CommentFavoriteModel: BaseModel
{
    public required string CommentId { get; set; }
    public required string UserId { get; set; }
    
    public CommentModel? Comment { get; set; }
    public UserModel? User { get; set; }
}