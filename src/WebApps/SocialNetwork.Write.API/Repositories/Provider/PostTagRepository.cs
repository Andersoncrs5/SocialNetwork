using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Repositories.Provider;

public class PostTagRepository(AppDbContext context, IRedisService redisService): GenericRepository<PostTagModel>(context, redisService), IPostTagRepository
{
    public async Task<int> CountByPostIdAndTagId([IsId] string postId, [IsId] string tagId)
        => await context.PostTags.CountAsync(p => p.PostId == postId && p.TagId == tagId);
    
    public async Task<PostTagModel?> GetByPostIdAndTagId([IsId] string postId, [IsId] string tagId)
        => await context.PostTags.AsNoTracking().FirstOrDefaultAsync(p => p.PostId == postId && p.TagId == tagId);
    
    
}