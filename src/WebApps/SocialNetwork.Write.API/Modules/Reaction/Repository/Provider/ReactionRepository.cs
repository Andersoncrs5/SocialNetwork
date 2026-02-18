using Microsoft.EntityFrameworkCore;
using SocialNetwork.Write.API.Configs.DB;
using SocialNetwork.Write.API.Modules.Reaction.Model;
using SocialNetwork.Write.API.Modules.Reaction.Repository.Interface;
using SocialNetwork.Write.API.Repositories.Provider;
using SocialNetwork.Write.API.Services.Interfaces;

namespace SocialNetwork.Write.API.Modules.Reaction.Repository.Provider;

public class ReactionRepository(AppDbContext context, IRedisService redisService): GenericRepository<ReactionModel>(context, redisService), IReactionRepository
{
    public async Task<bool> ExistsByName(string name)
        => await context.Reactions.AnyAsync(x => x.Name == name);
    
}