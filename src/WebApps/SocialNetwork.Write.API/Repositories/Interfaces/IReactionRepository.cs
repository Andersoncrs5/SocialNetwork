using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Repositories.Interfaces;

public interface IReactionRepository: IGenericRepository<ReactionModel>
{
    Task<bool> ExistsByName(string name);
}