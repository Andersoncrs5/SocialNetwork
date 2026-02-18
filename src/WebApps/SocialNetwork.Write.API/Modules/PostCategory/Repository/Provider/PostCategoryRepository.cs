using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.PostCategory.Model;
using SocialNetwork.Write.API.Modules.PostCategory.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostCategory.Repository.Provider;

public class PostCategoryRepository(AppDbContext app, IRedisService redisService)
    : GenericRepository<PostCategoryModel>(app, redisService), IPostCategoryRepository
{
    public async Task<bool> ExistsByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId)
        => await app.PostCategories.AnyAsync(x => x.PostId == postId && x.CategoryId == categoryId);
    
    public async Task<int> CountByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId)
        => await app.PostCategories.CountAsync(x => x.PostId == postId && x.CategoryId == categoryId);
    
    public async Task<PostCategoryModel?> GetByPostIdAndCategoryId([IsId] string postId, [IsId] string categoryId)
        => await app.PostCategories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PostId == postId && x.CategoryId == categoryId);
}