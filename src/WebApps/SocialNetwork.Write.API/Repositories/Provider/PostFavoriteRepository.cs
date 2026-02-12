using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Repositories.Provider;

public class PostFavoriteRepository(AppDbContext context, IRedisService redisService): GenericRepository<PostFavoriteModel>(context, redisService), IPostFavoriteRepository
{
    public async Task<bool> ExistsByPostIdAndUserId([IsId] string postId, [IsId] string userId)
        => await context.PostFavorites.AnyAsync(x => x.PostId == postId && x.UserId == userId);
    
    public async Task<PostFavoriteModel?> GetByPostIdAndUserId([IsId] string postId, [IsId] string userId)
        => await context.PostFavorites.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
}