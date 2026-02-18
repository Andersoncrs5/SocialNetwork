using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.Tag.Model;
using SocialNetwork.Write.API.Modules.Tag.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Tag.Repository.Provider;

public class TagRepository(AppDbContext context, IRedisService redisService): GenericRepository<TagModel>(context, redisService), ITagRepository
{
    public async Task<bool> ExistsBySlug([SlugConstraint] string slug)
        => await context.Categories.AnyAsync(c => c.Slug == slug);
    
    public async Task<TagModel?> FindBySlug([SlugConstraint] string slug)
        => await context.Tags.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug);
    
    public async Task<bool> ExistsByName([SlugConstraint] string name)
        => await context.Categories.AnyAsync(c => c.Name == name);

}