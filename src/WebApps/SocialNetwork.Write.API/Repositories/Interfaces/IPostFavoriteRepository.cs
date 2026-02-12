using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface IPostFavoriteRepository: IGenericRepository<PostFavoriteModel>
{
    Task<bool> ExistsByPostIdAndUserId([IsId] string postId, [IsId] string userId);
    Task<PostFavoriteModel?> GetByPostIdAndUserId([IsId] string postId, [IsId] string userId);
}