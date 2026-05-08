using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.CommentVote.Model;
using SocialNetwork.Write.API.Modules.CommentVote.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.CommentVote.Repository.Provider;

public class CommentVoteRepository(AppDbContext context, IRedisService redisService): GenericRepository<CommentVoteModel>(context, redisService), ICommentVoteRepository
{
    public async Task<CommentVoteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId) 
        => await context.CommentVoteModels.FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);
}