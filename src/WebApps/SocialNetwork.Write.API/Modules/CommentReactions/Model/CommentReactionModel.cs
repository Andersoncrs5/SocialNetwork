using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.Reaction.Model;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils.Bases;

namespace SocialNetwork.Write.API.Modules.CommentReactions.Model;

public class CommentReactionModel: BaseModel
{
    public required string UserId { get; set; }
    public required string ReactionId { get; set; } 
    public required string CommentId { get; set; }
    
    public UserModel? User { get; set; }
    public ReactionModel? Reaction { get; set; }
    public CommentModel? Comment { get; set; }
}