using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostCategory.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostCategory.Repository.Interface;

public interface IPostCategoryRepository: IGenericRepository<PostCategoryModel>
{
    Task<bool> ExistsByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId);
    Task<PostCategoryModel?> GetByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId);
    Task<int> CountByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId);
}