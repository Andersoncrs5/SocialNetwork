using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.CommentFavorite.Model;
using SocialNetwork.Write.API.Modules.CommentFavorite.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.CommentFavorite.Repository.Provider;

public class CommentFavoriteRepository(AppDbContext dbContext, IRedisService redisService) 
    : GenericRepository<CommentFavoriteModel>(dbContext, redisService), ICommentFavoriteRepository
{
    public async Task<CommentFavoriteModel?> GetByCommentIdAndUserId([IsId] string commentId, [IsId] string userId)
    {
        return await dbContext.Set<CommentFavoriteModel>()
            .AsNoTracking() 
            .FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);
    }
}