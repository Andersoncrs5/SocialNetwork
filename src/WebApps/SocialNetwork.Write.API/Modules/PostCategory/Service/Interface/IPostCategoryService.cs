using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Modules.PostCategory.Dto;
using SocialNetwork.Write.API.Modules.PostCategory.Model;

namespace SocialNetwork.Write.API.Modules.PostCategory.Service.Interface;

public interface IPostCategoryService
{
    Task<PostCategoryModel> GetByIdSimple([IsId] string id);
    Task<PostCategoryModel> GetByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId);
    Task Delete(PostCategoryModel postCategory, bool commit = true);
    Task<PostCategoryModel> Create(CreatePostCategoryDto dto, bool commit = true);
    Task<PostCategoryModel> Update(UpdatePostCategoryDto dto, PostCategoryModel postCategory, bool commit = true);
    Task<int> CountByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId);
}