using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils.Bases;
using SocialNetwork.Write.API.Utils.Enums;

namespace SocialNetwork.Write.API.Modules.PostVote.Model;

public class PostVoteModel: BaseModel
{
    public required string PostId { get; set; }
    public required string UserId { get; set; }
    
    public VoteEnum Vote { get; set; }
    
    public PostModel? Post { get; set; }
    public UserModel? User { get; set; }
}