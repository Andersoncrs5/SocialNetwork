using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Models.Enums.Post;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.Classes;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class PostFavoriteService(IUnitOfWork uow): IPostFavoriteService
{
    public async Task<PostFavoriteModel?> GetByPostIdAndUserId([IsId] string postId, [IsId] string userId)
        => await uow.PostFavoriteRepository.GetByPostIdAndUserId(postId, userId);

    public async Task DeleteAsync(PostFavoriteModel favorite, bool commit = true)
    {
        await uow.PostFavoriteRepository.DeleteAsync(favorite);
        if (commit) await uow.CommitAsync();
    }

    public async Task<PostFavoriteModel> CreateAsync(PostModel post, UserModel user, bool commit = true)
    {
        PostFavoriteModel favorite = new PostFavoriteModel()
        {
            PostId = post.Id,
            UserId = user.Id,
            Post = post,
            User = user
        };

        PostFavoriteModel favoriteAdded = await uow.PostFavoriteRepository.AddAsync(favorite);
        if (commit) await uow.CommitAsync();
        
        return favoriteAdded;
    }

    public async Task<ResultToggle<PostFavoriteModel>> ToggleAsync(PostModel post, UserModel user)
    {
        ResultToggle<PostFavoriteModel> result = new ResultToggle<PostFavoriteModel>();

        await uow.ExecuteTransactionAsync(async () =>
        {
            PostFavoriteModel? favorite = await uow.PostFavoriteRepository.GetByPostIdAndUserId(post.Id, user.Id);

            if (favorite is not null)
            {
                await uow.PostFavoriteRepository.DeleteAsync(favorite);
                result.Action = AddedORRemoved.Removed;
                result.Value = favorite;
            }
            else
            {
                PostFavoriteModel newFavorite = new PostFavoriteModel 
                { 
                    PostId = post.Id, 
                    UserId = user.Id 
                };

                PostFavoriteModel model = await uow.PostFavoriteRepository.AddAsync(newFavorite);
                result.Action = AddedORRemoved.Added;
                result.Value = model;
            }
        });

        return result;
    }
    
}