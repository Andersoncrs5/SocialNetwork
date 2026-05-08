using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils.Bases;
using SocialNetwork.Write.API.Utils.Enums;

namespace SocialNetwork.Write.API.Modules.CommentVote.Model;

public class CommentVoteModel: BaseModel
{
    public required string CommentId { get; set; }
    public required string UserId { get; set; }
    
    public VoteEnum Vote { get; set; }
    
    public UserModel? User { get; set; }
    public CommentModel? Comment { get; set; }
}