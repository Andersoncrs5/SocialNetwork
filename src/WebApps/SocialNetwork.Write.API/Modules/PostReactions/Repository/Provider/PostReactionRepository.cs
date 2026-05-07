using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.PostReactions.Model;
using SocialNetwork.Write.API.Modules.PostReactions.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostReactions.Repository.Provider;

public class PostReactionRepository(AppDbContext context, IRedisService redisService): GenericRepository<PostReactionModel>(context, redisService), IPostReactionRepository
{
    public async Task<PostReactionModel?> GetByUserIdAndPostId([IsId] string userId, [IsId] string postId)
        => await context.PostReactions.FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);
}