using SocialNetwork.Write.API.Models.Bases;

namespace SocialNetwork.Write.API.Models;

public class CommentFavoriteModel: BaseModel
{
    public required string CommentId { get; set; }
    public required string UserId { get; set; }
    
    public CommentModel? Comment { get; set; }
    public UserModel? User { get; set; }
}