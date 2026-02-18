using Microsoft.EntityFrameworkCore;
using SocialNetwork.Contracts.Attributes.Globals;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Category.Model;
using SocialNetwork.Write.API.Modules.Category.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Category.Repository.Provider;

public class CategoryRepository(AppDbContext context, IRedisService redisService): GenericRepository<CategoryModel>(context, redisService), ICategoryRepository
{
    public async Task<CategoryModel?> FindBySlug([SlugConstraint] string slug)
        => await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == slug);

    public async Task<bool> ExistsBySlug([SlugConstraint] string slug)
        => await context.Categories.AsNoTracking().AnyAsync(x => x.Slug == slug);
    
    public async Task<CategoryModel?> FindByName(string name)
        => await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
    
    public async Task<bool> ExistsByName(string name)
        => await context.Categories.AsNoTracking().AnyAsync(x => x.Name == name);
    
}