using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Repositories.Interfaces;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Repositories.Provider;

public class TagRepository(AppDbContext context, IRedisService redisService): GenericRepository<TagModel>(context, redisService), ITagRepository
{
    public async Task<bool> ExistsBySlug([SlugConstraint] string slug)
        => await context.Categories.AnyAsync(c => c.Slug == slug);
    
    public async Task<TagModel?> FindBySlug([SlugConstraint] string slug)
        => await context.Tags.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug);
    
}