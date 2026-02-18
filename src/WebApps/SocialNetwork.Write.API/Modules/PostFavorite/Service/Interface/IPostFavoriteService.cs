using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.PostFavorite.Model;
using SocialNetwork.Write.API.Modules.User.Model;
using SocialNetwork.Write.API.Utils.Classes;

namespace SocialNetwork.Write.API.Modules.PostFavorite.Service.Interface;

public interface IPostFavoriteService
{
    Task<PostFavoriteModel?> GetByPostIdAndUserId([IsId] string postId, [IsId] string userId);
    Task DeleteAsync(PostFavoriteModel favorite, bool commit = true);
    Task<PostFavoriteModel> CreateAsync(PostModel post, UserModel user, bool commit = true);
    Task<ResultToggle<PostFavoriteModel>> ToggleAsync(PostModel post, UserModel user);
}