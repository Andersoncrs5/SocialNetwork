using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.Exception.classes;
using SocialNetwork.Write.API.dto.PostCategory;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Services.Interfaces;
using SocialNetwork.Write.API.Utils.UnitOfWork;

namespace SocialNetwork.Write.API.Services.Providers;

public class PostCategoryService(IUnitOfWork uow): IPostCategoryService
{
    public async Task<PostCategoryModel> GetByIdSimple([IsId] string id)
        => await uow.PostCategoryRepository.GetByIdAsync(id) 
           ?? throw new ModelNotFoundException("Category not found");

    public async Task<int> CountByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId)
        => await uow.PostCategoryRepository.CountByPostIdAndCategoryId(postId, categoryId);
    
    public async Task<PostCategoryModel> GetByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId)
        => await uow.PostCategoryRepository.GetByPostIdAndCategoryId(postId, categoryId) 
           ?? throw new ModelNotFoundException("Category not found");

    public async Task Delete(PostCategoryModel postCategory, bool commit = true)
    {
        await uow.PostCategoryRepository.DeleteAsync(postCategory);
        if (commit) await uow.CommitAsync();
    }

    public async Task<PostCategoryModel> Create(CreatePostCategoryDto dto, bool commit = true)
    {
        PostCategoryModel model = uow.Mapper.Map<PostCategoryModel>(dto);

        PostCategoryModel postCategoryAdded = await uow.PostCategoryRepository.AddAsync(model);
        if (commit) await uow.CommitAsync();
        
        return postCategoryAdded;
    }

    public async Task<PostCategoryModel> Update(UpdatePostCategoryDto dto, PostCategoryModel postCategory, bool commit = true)
    {
        if (dto.Order.HasValue)
            postCategory.Order = dto.Order.Value;

        PostCategoryModel postCategoryUpdated = await uow.PostCategoryRepository.Update(postCategory);
        if (commit) await uow.CommitAsync();
        
        return postCategoryUpdated;
    }
    
}