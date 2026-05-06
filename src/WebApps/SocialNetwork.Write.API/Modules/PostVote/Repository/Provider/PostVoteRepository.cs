using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.PostVote.Model;
using SocialNetwork.Write.API.Modules.PostVote.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.PostVote.Repository.Provider;

public class PostVoteRepository(AppDbContext context, IRedisService redisService): 
    GenericRepository<PostVoteModel>(context, redisService), IPostVoteRepository
{
    public async Task<PostVoteModel?> GetByPostIdAndUserId([IsId] string postId, [IsId] string userId) 
        => await context.PostVotes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
}