using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.CommentReactions.Repository.Interface;

public interface ICommentReactionRepository: IGenericRepository<CommentReactionModel>
{
    Task<bool> ExistsByUserIdAndCommentId([IsId] string userId, [IsId] string commentId);
    Task<CommentReactionModel?> GetByUserIdAndCommentId([IsId] string userId, [IsId] string commentId);
}