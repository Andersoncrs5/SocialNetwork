using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostFavorite.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostFavorite.Repository.Interface;

public interface IPostFavoriteRepository: IGenericRepository<PostFavoriteModel>
{
    Task<bool> ExistsByPostIdAndUserId([IsId] string postId, [IsId] string userId);
    Task<PostFavoriteModel?> GetByPostIdAndUserId([IsId] string postId, [IsId] string userId);
}