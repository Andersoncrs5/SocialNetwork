using SocialNetwork.Write.API.Modules.Reaction.Model;
using SocialNetwork.Write.API.Repositories.Interfaces;

namespace SocialNetwork.Write.API.Modules.Reaction.Repository.Interface;

public interface IReactionRepository: IGenericRepository<ReactionModel>
{
    Task<bool> ExistsByName(string name);
}