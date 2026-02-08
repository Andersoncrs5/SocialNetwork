using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface IPostCategoryRepository: IGenericRepository<PostCategoryModel>
{
    Task<bool> ExistsByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId);
    Task<PostCategoryModel?> GetByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId);
}