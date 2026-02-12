using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Utils.Classes;

namespace SocialNetwork.Write.API.Services.Interfaces;

public interface IPostFavoriteService
{
    Task<PostFavoriteModel?> GetByPostIdAndUserId([IsId] string postId, [IsId] string userId);
    Task DeleteAsync(PostFavoriteModel favorite, bool commit = true);
    Task<PostFavoriteModel> CreateAsync(PostModel post, UserModel user, bool commit = true);
    Task<ResultToggle<PostFavoriteModel>> ToggleAsync(PostModel post, UserModel user);
}