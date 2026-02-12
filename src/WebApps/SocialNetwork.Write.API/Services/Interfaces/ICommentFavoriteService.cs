using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface ICommentFavoriteService
{
    Task DeleteAsync(CommentFavoriteModel model, bool commit = true);
    Task<CommentFavoriteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId);
}