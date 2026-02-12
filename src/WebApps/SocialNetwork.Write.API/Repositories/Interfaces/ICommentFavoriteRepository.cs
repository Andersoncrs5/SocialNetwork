using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface ICommentFavoriteRepository: IGenericRepository<CommentFavoriteModel>
{
    Task<CommentFavoriteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId);
}