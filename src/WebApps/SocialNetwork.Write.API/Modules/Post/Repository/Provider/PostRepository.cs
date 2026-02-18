using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.Post.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Post.Repository.Provider;

public class PostRepository(AppDbContext app, IRedisService redisService): GenericRepository<PostModel>(app, redisService), IPostRepository
{
    public async Task<bool> ExistsBySlug([SlugConstraint] string slug) 
        => await app.Posts.AnyAsync(x => x.Slug == slug);
}