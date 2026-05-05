using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.CommentReactions.Model;
using SocialNetwork.Write.API.Modules.CommentReactions.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.CommentReactions.Repository.Provider;

public class CommentReactionRepository(AppDbContext context, IRedisService redisService): GenericRepository<CommentReactionModel>(context, redisService), ICommentReactionRepository
{
    public async Task<bool> ExistsByUserIdAndCommentId([IsId] string userId, [IsId] string commentId)
        => await context.CommentReactions.AnyAsync(x => x.UserId == userId && x.CommentId == commentId);
    
    public async Task<CommentReactionModel?> GetByUserIdAndCommentId([IsId] string userId, [IsId] string commentId)
        => await context.CommentReactions.FirstOrDefaultAsync(x => x.UserId == userId && x.CommentId == commentId);

}