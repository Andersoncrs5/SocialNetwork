using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Repositories.Provider;

public class CommentRepository(AppDbContext app, IRedisService redisService) : GenericRepository<CommentModel>(app, redisService), ICommentRepository
{
    
}