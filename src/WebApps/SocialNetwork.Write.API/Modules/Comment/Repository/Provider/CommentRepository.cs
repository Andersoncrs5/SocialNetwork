using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.Comment.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Comment.Repository.Provider;

public class CommentRepository(AppDbContext app, IRedisService redisService) : GenericRepository<CommentModel>(app, redisService), ICommentRepository
{
    
}