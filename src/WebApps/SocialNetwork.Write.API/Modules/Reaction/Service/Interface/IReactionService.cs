using SocialNetwork.Write.API.Modules.Reaction.Dto;
using SocialNetwork.Write.API.Modules.Reaction.Model;

namespace SocialNetwork.Write.API.Modules.Reaction.Service.Interface;

public interface IReactionService
{
    Task<ReactionModel> GetByIdAsync(string id);
    Task<bool> ExistsByName(string name);
    Task<ReactionModel> CreateAsync(CreateReactionDto dto, bool commit = true);
    Task DeleteAsync(ReactionModel reaction, bool commit = true);
    Task<ReactionModel> UpdateAsync(UpdateReactionDto dto, ReactionModel reaction, bool commit = true);
}