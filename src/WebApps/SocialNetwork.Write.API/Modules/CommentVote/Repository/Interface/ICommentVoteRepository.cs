using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.CommentVote.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.CommentVote.Repository.Interface;

public interface ICommentVoteRepository: IGenericRepository<CommentVoteModel>
{
    Task<CommentVoteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId);
}