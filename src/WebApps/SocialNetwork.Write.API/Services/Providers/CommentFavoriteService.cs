using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class CommentFavoriteService(IUnitOfWork uow): ICommentFavoriteService
{
    public async Task<CommentFavoriteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId)
        => await uow.CommentFavoriteRepository.GetByCommentIdAndUserId(commentId, userId);

    public async Task DeleteAsync(CommentFavoriteModel model, bool commit = true)
    {
        await uow.CommentFavoriteRepository.DeleteAsync(model);
        if (commit) await uow.CommitAsync();
    }

    public async Task<CommentFavoriteModel> CreateAsync([IsId] string commentId, [IsId] string userId)
    {
        CommentFavoriteModel model = new CommentFavoriteModel()
        {
            CommentId = commentId,
            UserId = userId,
        };

        CommentFavoriteModel commentFavorite = await uow.CommentFavoriteRepository.AddAsync(model);
        await uow.CommitAsync();
        return commentFavorite;
    }

    public async Task<ResultToggle<CommentFavoriteModel?>> ToggleAsync([IsId] string commentId, [IsId] string userId, bool commit = true)
    {
        
        CommentFavoriteModel? favorite = await uow.CommentFavoriteRepository.GetByCommentIdAndUserId(commentId, userId);

        if (favorite != null)
        {
            await uow.CommentFavoriteRepository.DeleteAsync(favorite);
            if (commit) await uow.CommitAsync();
            return new ResultToggle<CommentFavoriteModel?>()
            {
                Action = AddedORRemoved.Removed,
                Value = null
            };
        }

        CommentFavoriteModel model = new CommentFavoriteModel()
        {
            CommentId = commentId,
            UserId = userId,
        };

        CommentFavoriteModel addAsync = await uow.CommentFavoriteRepository.AddAsync(model);

        if (commit) await uow.CommitAsync();
        return new ResultToggle<CommentFavoriteModel?>()
        {
            Action = AddedORRemoved.Added,
            Value = addAsync
        };
    }
    
}