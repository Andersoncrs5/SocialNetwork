using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.CommentFavorite.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.CommentFavorite.Repository.Interface;

public interface ICommentFavoriteRepository: IGenericRepository<CommentFavoriteModel>
{
    Task<CommentFavoriteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId);
}