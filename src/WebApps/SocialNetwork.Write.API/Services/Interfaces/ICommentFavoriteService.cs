using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Utils.Classes;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface ICommentFavoriteService
{
    Task DeleteAsync(CommentFavoriteModel model, bool commit = true);
    Task<CommentFavoriteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId);
    Task<CommentFavoriteModel> CreateAsync([IsId] string commentId, [IsId] string userId);
    Task<ResultToggle<CommentFavoriteModel?>> ToggleAsync([IsId] string commentId, [IsId] string userId,
        bool commit = true);
}