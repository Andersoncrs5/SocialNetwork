using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Reaction.Model;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils.Bases;

namespace SocialNetwork.Write.API.Modules.PostReactions.Model;

public class PostReactionModel: BaseModel
{
    public required string UserId { get; set; }
    public required string ReactionId { get; set; }
    public required string PostId { get; set; }
    
    public UserModel? User { get; set; }
    public ReactionModel? Reaction { get; set; }
    public PostModel? Post { get; set; }
}