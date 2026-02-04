using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Services.Providers;

public class PostRepository(AppDbContext app, IRedisService redisService): GenericRepository<PostModel>(app, redisService), IPostRepository
{
    
}